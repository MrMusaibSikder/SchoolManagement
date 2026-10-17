import { useMemo, useState } from "react";
import type { AxiosError } from "axios";
import { Link } from "react-router-dom";
import { useQueryClient } from "@tanstack/react-query";
import {
  CalendarDays,
  CheckCircle2,
  ChevronLeft,
  ChevronRight,
  CircleDollarSign,
  Clock3,
  FileText,
  Loader2,
  Plus,
  ReceiptText,
  RotateCw,
  Sparkles,
  Users,
  X,
} from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useAcademicYears, useSchoolClasses } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { loadFeeStructure, useFeeStructures } from "@/features/fee-structure/hooks/useFeeStructureData";
import { useFeeTypes } from "@/features/fee-type/hooks/useFeeTypeData";
import { useApplicableStudentConcessions } from "@/features/student-fee-concession/hooks/useStudentFeeConcessionData";
import { ConcessionType } from "@/features/student-fee-concession/types/student-fee-concession.types";
import { useStudents } from "@/features/student/hooks/useStudentData";
import { Permission } from "@/lib/permissions";
import { useApplyLateFines, useCancelInvoice, useCreateInvoice, useGenerateMonthlyInvoices, useInvoice, useInvoices } from "../hooks/useInvoiceData";
import { InvoiceStatus, type InvoiceFilters, type InvoiceGenerationResultDto, type InvoiceStatusValue, type LateFineApplicationResultDto } from "../types/invoice.types";

const PAGE_SIZE = 20;
const statuses = ["Draft", "Issued", "Partially paid", "Paid", "Overdue", "Cancelled"];

interface DraftItem {
  feeTypeId: string;
  description: string;
  originalAmount: string;
  fineAmount: string;
  quantity: string;
  sortOrder: number;
}

function localDate(date: Date) {
  return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}-${String(date.getDate()).padStart(2, "0")}`;
}

function datePlus(days: number) {
  const date = new Date();
  date.setDate(date.getDate() + days);
  return localDate(date);
}

function money(amount: number) {
  return new Intl.NumberFormat("en-BD", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(amount);
}

function statusLabel(value: number) {
  return statuses[value - 1] ?? "Unknown";
}

function statusClass(value: number) {
  if (value === InvoiceStatus.Paid) return "border-emerald-200 bg-emerald-50 text-emerald-800 dark:border-emerald-900 dark:bg-emerald-950/50 dark:text-emerald-200";
  if (value === InvoiceStatus.Overdue) return "border-rose-200 bg-rose-50 text-rose-800 dark:border-rose-900 dark:bg-rose-950/50 dark:text-rose-200";
  if (value === InvoiceStatus.Cancelled) return "border-slate-200 bg-slate-100 text-slate-700 dark:border-slate-800 dark:bg-slate-900 dark:text-slate-300";
  if (value === InvoiceStatus.PartiallyPaid) return "border-amber-200 bg-amber-50 text-amber-800 dark:border-amber-900 dark:bg-amber-950/50 dark:text-amber-200";
  if (value === InvoiceStatus.Issued) return "border-sky-200 bg-sky-50 text-sky-800 dark:border-sky-900 dark:bg-sky-950/50 dark:text-sky-200";
  return "border-violet-200 bg-violet-50 text-violet-800 dark:border-violet-900 dark:bg-violet-950/50 dark:text-violet-200";
}

function statusAccentClass(value: number) {
  if (value === InvoiceStatus.Paid) return "border-l-emerald-500";
  if (value === InvoiceStatus.Overdue) return "border-l-rose-500";
  if (value === InvoiceStatus.Cancelled) return "border-l-slate-400";
  if (value === InvoiceStatus.PartiallyPaid) return "border-l-amber-400";
  if (value === InvoiceStatus.Issued) return "border-l-sky-500";
  return "border-l-violet-400";
}

function errorMessage(error: unknown, fallback: string) {
  const response = (error as AxiosError<{ message?: string } | string>).response;
  if (typeof response?.data === "string" && response.data) return response.data;
  if (response?.data && typeof response.data === "object" && response.data.message) return response.data.message;
  if (response?.status === 409) return "This invoice changed concurrently. Latest data has been refreshed; review it before retrying.";
  return fallback;
}

function discountFor(amount: number, type: number, value: number | null) {
  if (type === ConcessionType.FullExemption) return amount;
  if (value === null) return 0;
  if (type === ConcessionType.PercentageDiscount) return amount * value / 100;
  return Math.min(amount, value);
}

export function InvoicesPage() {
  const { hasPermission } = usePermissions();
  const canView = hasPermission(Permission.InvoiceView);
  const canCreate = hasPermission(Permission.InvoiceCreate);
  const canCancel = hasPermission(Permission.InvoiceCancel);
  const canApplyLateFines = canCreate;
  const canViewConcessions = hasPermission(Permission.ConcessionView);
  const canManagePayments = hasPermission(Permission.PaymentView) || hasPermission(Permission.PaymentCollect);
  const canViewFeeReports = hasPermission(Permission.FeeReportView);
  const { data: students = [] } = useStudents();
  const { data: years = [] } = useAcademicYears();
  const { data: classes = [] } = useSchoolClasses();
  const { data: feeTypes = [] } = useFeeTypes();

  const [filters, setFilters] = useState<InvoiceFilters>({ pageNumber: 1, pageSize: PAGE_SIZE });
  const { data: invoicePage, isPending, isError } = useInvoices(filters);
  const [selectedId, setSelectedId] = useState<number | null>(null);
  const { data: invoice, isPending: detailPending, isError: detailError } = useInvoice(selectedId);
  const queryClient = useQueryClient();

  const [studentId, setStudentId] = useState("");
  const [yearId, setYearId] = useState("");
  const [structureId, setStructureId] = useState("");
  const [invoiceDate, setInvoiceDate] = useState(localDate(new Date()));
  const [dueDate, setDueDate] = useState(datePlus(7));
  const [billingMonth, setBillingMonth] = useState("");
  const [billingYear, setBillingYear] = useState(String(new Date().getFullYear()));
  const [notes, setNotes] = useState("");
  const [items, setItems] = useState<DraftItem[]>([]);
  const [loadingStructure, setLoadingStructure] = useState(false);

  const currentStudent = students.find((student) => String(student.id) === studentId);
  const { data: concessions = [], isPending: concessionsPending, isError: concessionsError } = useApplicableStudentConcessions(
    studentId && canViewConcessions ? Number(studentId) : null,
    canViewConcessions
  );
  const { data: structures = [] } = useFeeStructures({
    academicYearId: yearId ? Number(yearId) : undefined,
    schoolClassId: currentStudent?.classId,
    isActive: true,
  });

  const createMutation = useCreateInvoice();
  const cancelMutation = useCancelInvoice();
  const generationMutation = useGenerateMonthlyInvoices();
  const applyLateFinesMutation = useApplyLateFines();

  const [generationYear, setGenerationYear] = useState("");
  const [generationMonth, setGenerationMonth] = useState(String(new Date().getMonth() + 1));
  const [generationCalendarYear, setGenerationCalendarYear] = useState(String(new Date().getFullYear()));
  const [generationClass, setGenerationClass] = useState("");
  const [generationInvoiceDate, setGenerationInvoiceDate] = useState(localDate(new Date()));
  const [generationDueDate, setGenerationDueDate] = useState(datePlus(7));
  const [generationResult, setGenerationResult] = useState<InvoiceGenerationResultDto | null>(null);
  const [asOfDate, setAsOfDate] = useState("");
  const [lateFineResult, setLateFineResult] = useState<LateFineApplicationResultDto | null>(null);

  const computedItems = useMemo(() => items.map((item) => {
    const originalAmount = Number(item.originalAmount) || 0;
    const concession = concessions.find((entry) => {
      if (!entry.isApproved || !entry.isActive || entry.feeTypeId !== Number(item.feeTypeId)) return false;
      if (entry.academicYearId !== Number(yearId)) return false;
      if (entry.validFrom && entry.validFrom.slice(0, 10) > invoiceDate) return false;
      if (entry.validTo && entry.validTo.slice(0, 10) < invoiceDate) return false;
      return true;
    });
    const discountAmount = concession ? discountFor(originalAmount, concession.type, concession.value ?? null) : 0;
    const fineAmount = Number(item.fineAmount) || 0;
    const quantity = Number(item.quantity) || 0;
    return { ...item, originalAmount, discountAmount, fineAmount, quantity, netAmount: originalAmount - discountAmount + fineAmount };
  }), [items, concessions, yearId, invoiceDate]);
  const total = computedItems.reduce((sum, item) => sum + item.netAmount * item.quantity, 0);

  function changeFilter<K extends keyof InvoiceFilters>(key: K, value: InvoiceFilters[K] | undefined) {
    setFilters((current) => ({ ...current, [key]: value, pageNumber: 1 }));
  }

  function addItem(feeTypeId = "") {
    const selectedType = feeTypes.find((type) => String(type.id) === feeTypeId);
    setStructureId("");
    setItems((current) => [...current, {
      feeTypeId,
      description: selectedType?.name ?? "",
      originalAmount: "",
      fineAmount: "0",
      quantity: "1",
      sortOrder: current.length + 1,
    }]);
  }

  async function selectStructure(id: string) {
    setStructureId(id);
    setItems([]);
    if (!id) return;
    setLoadingStructure(true);
    try {
      const structure = await queryClient.fetchQuery({
        queryKey: ["fee-structure", "detail", Number(id)],
        queryFn: () => loadFeeStructure(Number(id)),
        staleTime: 30_000,
      });
      setItems(structure.items.filter((item) => !item.isOptional).map((item, index) => ({
        feeTypeId: String(item.feeTypeId),
        description: item.feeTypeName ?? feeTypes.find((type) => type.id === item.feeTypeId)?.name ?? "Fee item",
        originalAmount: String(item.amount),
        fineAmount: "0",
        quantity: "1",
        sortOrder: item.sortOrder || index + 1,
      })));
    } catch (error) {
      toast.error(errorMessage(error, "Unable to load fee structure items."));
    } finally {
      setLoadingStructure(false);
    }
  }

  async function submitInvoice(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!canViewConcessions) return toast.error("Concession view permission is required to apply student discounts safely.");
    if (concessionsPending || concessionsError) return toast.error("Concession details must load successfully before creating this invoice.");
    if (!studentId || !yearId || !invoiceDate || !dueDate) return toast.error("Complete the required invoice details.");
    if (dueDate < invoiceDate) return toast.error("Due date cannot be earlier than invoice date.");
    if (notes.length > 500) return toast.error("Notes must be 500 characters or fewer.");
    if (!computedItems.length || computedItems.some((item) => !item.feeTypeId || !item.description.trim() || item.originalAmount <= 0 || item.fineAmount < 0 || item.quantity < 1)) {
      return toast.error("Add valid invoice items with positive amounts and quantities.");
    }
    try {
      const created = await createMutation.mutateAsync({
        studentId: Number(studentId),
        academicYearId: Number(yearId),
        feeStructureId: structureId ? Number(structureId) : null,
        invoiceDate,
        dueDate,
        month: billingMonth ? Number(billingMonth) : null,
        year: billingMonth ? Number(billingYear) : null,
        notes: notes.trim() || null,
        items: computedItems.map((item) => ({
          feeTypeId: Number(item.feeTypeId),
          description: item.description.trim(),
          originalAmount: item.originalAmount,
          discountAmount: item.discountAmount,
          fineAmount: item.fineAmount,
          quantity: item.quantity,
          sortOrder: item.sortOrder,
        })),
      });
      toast.success(`Invoice ${created.invoiceNumber} created.`);
      setSelectedId(created.id);
      setItems([]);
      setStructureId("");
    } catch (error) {
      toast.error(errorMessage(error, "Unable to create invoice."));
    }
  }

  async function submitMonthly(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!generationYear || !generationMonth || !generationCalendarYear || !generationDueDate) return toast.error("Complete the required monthly generation details.");
    try {
      const result = await generationMutation.mutateAsync({
        academicYearId: Number(generationYear),
        month: Number(generationMonth),
        year: Number(generationCalendarYear),
        schoolClassId: generationClass ? Number(generationClass) : null,
        dueDate: generationDueDate,
        invoiceDate: generationInvoiceDate || null,
      });
      setGenerationResult(result);
      toast.success(result.failed ? "Generation finished with partial failures." : "Monthly invoice generation finished.");
    } catch (error) {
      toast.error(errorMessage(error, "Unable to generate monthly invoices."));
    }
  }

  async function handleApplyLateFines() {
    try {
      const result = await applyLateFinesMutation.mutateAsync(asOfDate || undefined);
      setLateFineResult(result);
      toast.success(result.failed ? "Late fine recalculation finished with failures." : "Late fine recalculation finished.");
    } catch (error) {
      toast.error(errorMessage(error, "Unable to apply late fines."));
    }
  }

  async function cancelSelectedInvoice() {
    if (!invoice || invoice.amountPaid !== 0 || !canCancel) return;
    const cancellationReason = window.prompt("Enter the cancellation reason:")?.trim();
    if (!cancellationReason) return;
    try {
      await cancelMutation.mutateAsync({ id: invoice.id, payload: { cancellationReason } });
      toast.success("Invoice cancelled.");
    } catch (error) {
      toast.error(errorMessage(error, "Unable to cancel invoice."));
    }
  }

  if (!canView && !canCreate) return <Card><CardContent className="p-6 text-sm text-destructive">You do not have permission to access invoices.</CardContent></Card>;

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <Card className="overflow-hidden border-primary/20 bg-gradient-to-br from-primary to-primary/90 text-primary-foreground shadow-lg shadow-primary/10">
        <CardContent className="flex flex-col gap-5 p-5 sm:flex-row sm:items-center sm:justify-between sm:p-7">
          <div className="flex items-start gap-4">
            <span className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl border border-white/15 bg-white/10">
              <ReceiptText aria-hidden="true" className="h-6 w-6" />
            </span>
            <div>
              <p className="text-xs font-semibold uppercase tracking-[0.2em] text-primary-foreground/70">
                Fee management
              </p>
              <h1 className="mt-1 font-display text-2xl font-semibold sm:text-3xl">
                Invoice register
              </h1>
              <p className="mt-2 max-w-2xl text-sm leading-6 text-primary-foreground/80">
                Create student bills, track payments and outstanding balances,
                and manage monthly billing from one workspace.
              </p>
            </div>
          </div>
          {(canManagePayments || canViewFeeReports) && (
            <div className="flex flex-wrap gap-2 sm:shrink-0">
              {canManagePayments && <Link
                to="/fees/payments"
                className="inline-flex items-center justify-center gap-2 rounded-md border border-white/20 bg-white/10 px-4 py-2 text-sm font-medium text-primary-foreground transition hover:bg-white/20"
              >
                <CircleDollarSign aria-hidden="true" className="h-4 w-4" />
                Payments
              </Link>}
              {canViewFeeReports && <Link
                to="/fees/reports"
                className="inline-flex items-center justify-center gap-2 rounded-md bg-background px-4 py-2 text-sm font-semibold text-primary shadow-sm transition hover:bg-background/90"
              >
                <FileText aria-hidden="true" className="h-4 w-4" />
                Fee reports
              </Link>}
            </div>
          )}
        </CardContent>
      </Card>

      {canCreate && <div className="grid gap-6 xl:grid-cols-2">
        <Card className="border-t-4 border-t-emerald-500">
          <CardHeader>
            <CardTitle className="flex items-center gap-3">
              <span className="flex h-10 w-10 items-center justify-center rounded-lg bg-emerald-100 text-emerald-800 dark:bg-emerald-900/70 dark:text-emerald-200">
                <Plus aria-hidden="true" className="h-5 w-5" />
              </span>
              Create invoice
            </CardTitle>
            <CardDescription className="sm:pl-[3.25rem]">
              Header totals are assigned by the server. Eligible concessions
              are applied from student records.
            </CardDescription>
          </CardHeader>
          <CardContent>
            {!canViewConcessions ? <p className="rounded-md border border-amber-500/30 bg-amber-500/5 p-4 text-sm">Concession view permission is required to verify and apply student discounts.</p> : <form onSubmit={(event) => void submitInvoice(event)} className="space-y-5">
              <div className="grid gap-3 sm:grid-cols-2">
                <div><label className="mb-1 block text-sm font-medium">Student</label><select required value={studentId} onChange={(event) => { setStudentId(event.target.value); setStructureId(""); setItems([]); }} className="w-full rounded-md border bg-background px-3 py-2"><option value="">Select student</option>{students.map((student) => <option key={student.id} value={student.id}>{student.fullName} · {student.admissionNumber}</option>)}</select></div>
                <div><label className="mb-1 block text-sm font-medium">Academic year</label><select required value={yearId} onChange={(event) => { setYearId(event.target.value); setStructureId(""); setItems([]); }} className="w-full rounded-md border bg-background px-3 py-2"><option value="">Select year</option>{years.map((year) => <option key={year.id} value={year.id}>{year.name}</option>)}</select></div>
                <div><label className="mb-1 block text-sm font-medium">Fee structure <span className="text-muted-foreground">(optional)</span></label><select disabled={!currentStudent || !yearId} value={structureId} onChange={(event) => void selectStructure(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2"><option value="">Add items manually</option>{structures.filter((item) => !item.sectionId || item.sectionId === currentStudent?.sectionId).map((item) => <option key={item.id} value={item.id}>{item.name}{item.sectionName ? ` · ${item.sectionName}` : " · All sections"}</option>)}</select></div>
                <div className="grid grid-cols-2 gap-2"><div><label className="mb-1 block text-sm font-medium">Invoice date</label><input type="date" required value={invoiceDate} onChange={(event) => setInvoiceDate(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div><div><label className="mb-1 block text-sm font-medium">Due date</label><input type="date" min={invoiceDate} required value={dueDate} onChange={(event) => setDueDate(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div></div>
                <div className="grid grid-cols-2 gap-2"><div><label className="mb-1 block text-sm font-medium">Billing month</label><select value={billingMonth} onChange={(event) => setBillingMonth(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2"><option value="">Not monthly</option>{Array.from({ length: 12 }, (_, index) => <option key={index + 1} value={index + 1}>{new Date(2000, index).toLocaleString(undefined, { month: "long" })}</option>)}</select></div><div><label className="mb-1 block text-sm font-medium">Billing year</label><input type="number" min="2000" max="2100" disabled={!billingMonth} value={billingYear} onChange={(event) => setBillingYear(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div></div>
              </div>

              {studentId && yearId && concessionsError && <p className="text-sm text-destructive">Concessions could not be verified. Invoice creation is disabled until the lookup succeeds.</p>}
              <div className="space-y-3 rounded-xl border bg-muted/20 p-4">
                <div className="flex items-center justify-between"><div><h3 className="text-sm font-semibold">Invoice items</h3><p className="text-xs text-muted-foreground">Discounts are read-only and calculated from active, approved concessions.</p></div><Button type="button" variant="outline" className="px-3 py-1.5" disabled={!feeTypes.length} onClick={() => addItem()}><Plus className="mr-2 h-4 w-4" />Add item</Button></div>
                {loadingStructure && <p className="flex items-center text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading fee structure…</p>}
                {items.map((item, index) => {
                  const computed = computedItems[index];
                  return <div key={`${item.sortOrder}-${index}`} className="grid gap-3 rounded-lg border bg-card p-3 shadow-sm sm:grid-cols-[1.4fr_1fr_auto] sm:items-end">
                    <div><label className="mb-1 block text-xs font-medium">Fee type</label><select required value={item.feeTypeId} onChange={(event) => { const feeType = feeTypes.find((type) => String(type.id) === event.target.value); setItems((current) => current.map((value, itemIndex) => itemIndex === index ? { ...value, feeTypeId: event.target.value, description: feeType?.name ?? "" } : value)); }} className="w-full rounded-md border bg-background px-3 py-2 text-sm"><option value="">Select fee type</option>{feeTypes.filter((type) => type.isActive || type.id === Number(item.feeTypeId)).map((type) => <option key={type.id} value={type.id}>{type.name} · {type.code}</option>)}</select></div>
                    <div className="grid grid-cols-2 gap-2"><div><label className="mb-1 block text-xs font-medium">Amount</label><input type="number" required min="0.01" step="0.01" value={item.originalAmount} onChange={(event) => setItems((current) => current.map((value, itemIndex) => itemIndex === index ? { ...value, originalAmount: event.target.value } : value))} className="w-full rounded-md border bg-background px-3 py-2 text-sm" /></div><div><label className="mb-1 block text-xs font-medium">Qty</label><input type="number" required min="1" step="1" value={item.quantity} onChange={(event) => setItems((current) => current.map((value, itemIndex) => itemIndex === index ? { ...value, quantity: event.target.value } : value))} className="w-full rounded-md border bg-background px-3 py-2 text-sm" /></div><div><label className="mb-1 block text-xs font-medium">Fine</label><input type="number" min="0" step="0.01" value={item.fineAmount} onChange={(event) => setItems((current) => current.map((value, itemIndex) => itemIndex === index ? { ...value, fineAmount: event.target.value } : value))} className="w-full rounded-md border bg-background px-3 py-2 text-sm" /></div><p className="self-end pb-2 text-xs text-muted-foreground">Discount {money(computed.discountAmount)}</p></div>
                    <Button type="button" variant="outline" className="h-9 w-9 px-0 py-0" aria-label="Remove invoice item" onClick={() => setItems((current) => current.filter((_, itemIndex) => itemIndex !== index))}><X className="h-4 w-4" /></Button>
                    <div className="sm:col-span-3"><label className="mb-1 block text-xs font-medium">Description</label><input required maxLength={200} value={item.description} onChange={(event) => setItems((current) => current.map((value, itemIndex) => itemIndex === index ? { ...value, description: event.target.value } : value))} className="w-full rounded-md border bg-background px-3 py-2 text-sm" /></div>
                  </div>;
                })}
                {!items.length && <p className="rounded-md border border-dashed p-4 text-sm text-muted-foreground">Select a fee structure or add an invoice item.</p>}
                <div className="flex flex-wrap items-center justify-between gap-2 rounded-lg border border-emerald-200 bg-emerald-50 p-4 text-sm dark:border-emerald-900 dark:bg-emerald-950/40"><span className="font-medium text-emerald-900 dark:text-emerald-100">Estimated invoice total</span><strong className="text-lg tabular-nums text-emerald-900 dark:text-emerald-100">BDT {money(total)}</strong></div>
              </div>
              <div><label className="mb-1 block text-sm font-medium">Notes</label><textarea maxLength={500} rows={2} value={notes} onChange={(event) => setNotes(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div>
              <Button type="submit" disabled={createMutation.isPending || loadingStructure || concessionsPending || concessionsError || !items.length}>{createMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <FileText className="mr-2 h-4 w-4" />}Create invoice</Button>
            </form>}
          </CardContent>
        </Card>

        <Card className="border-t-4 border-t-sky-500">
          <CardHeader>
            <CardTitle className="flex items-center gap-3">
              <span className="flex h-10 w-10 items-center justify-center rounded-lg bg-sky-100 text-sky-800 dark:bg-sky-900/70 dark:text-sky-200">
                <CalendarDays aria-hidden="true" className="h-5 w-5" />
              </span>
              Generate monthly invoices
            </CardTitle>
            <CardDescription className="sm:pl-[3.25rem]">
              Existing invoices are skipped; review any partial failures
              returned by the server.
            </CardDescription>
          </CardHeader>
          <CardContent><form onSubmit={(event) => void submitMonthly(event)} className="space-y-4">
            <div className="grid gap-3 sm:grid-cols-2">
              <div><label className="mb-1 block text-sm font-medium">Academic year</label><select required value={generationYear} onChange={(event) => setGenerationYear(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2"><option value="">Select year</option>{years.map((year) => <option key={year.id} value={year.id}>{year.name}</option>)}</select></div>
              <div><label className="mb-1 block text-sm font-medium">Class <span className="text-muted-foreground">(optional)</span></label><select value={generationClass} onChange={(event) => setGenerationClass(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2"><option value="">All classes</option>{classes.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}</select></div>
              <div><label className="mb-1 block text-sm font-medium">Month</label><select required value={generationMonth} onChange={(event) => setGenerationMonth(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2">{Array.from({ length: 12 }, (_, index) => <option key={index + 1} value={index + 1}>{new Date(2000, index).toLocaleString(undefined, { month: "long" })}</option>)}</select></div>
              <div><label className="mb-1 block text-sm font-medium">Calendar year</label><input required type="number" min="2000" max="2100" value={generationCalendarYear} onChange={(event) => setGenerationCalendarYear(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div>
              <div><label className="mb-1 block text-sm font-medium">Invoice date <span className="text-muted-foreground">(optional)</span></label><input type="date" value={generationInvoiceDate} onChange={(event) => setGenerationInvoiceDate(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div>
              <div><label className="mb-1 block text-sm font-medium">Due date</label><input required type="date" value={generationDueDate} onChange={(event) => setGenerationDueDate(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div>
            </div>
            <Button type="submit" disabled={generationMutation.isPending}>{generationMutation.isPending && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}Generate monthly invoices</Button>
          </form>
          {generationResult && <div className="mt-5 space-y-3 rounded-xl border bg-muted/20 p-4"><h3 className="flex items-center gap-2 text-sm font-semibold"><CheckCircle2 className="h-4 w-4 text-emerald-600" />Generation result</h3><div className="grid grid-cols-2 gap-3 text-sm sm:grid-cols-3">{[["Evaluated", generationResult.totalStudentsEvaluated, "text-foreground"], ["Created", generationResult.invoicesCreated, "text-emerald-700"], ["Already invoiced", generationResult.skippedAlreadyInvoiced, "text-sky-700"], ["No monthly items", generationResult.skippedNoMonthlyItems, "text-amber-700"], ["Failed", generationResult.failed, "text-rose-700"]].map(([label, value, tone]) => <div key={label} className="rounded-lg border bg-card p-3"><p className="text-xs text-muted-foreground">{label}</p><p className={`mt-1 text-lg font-semibold tabular-nums ${tone}`}>{value}</p></div>)}</div>{generationResult.errors.map((error) => <p key={`${error.studentId}-${error.reason}`} className="text-sm text-destructive">{error.studentName}: {error.reason}</p>)}</div>}
          </CardContent>
        </Card>
      </div>}

      {canApplyLateFines && <Card className="border-t-4 border-t-amber-400">
        <CardHeader>
          <CardTitle className="flex items-center gap-3">
            <span className="flex h-10 w-10 items-center justify-center rounded-lg bg-amber-100 text-amber-800 dark:bg-amber-900/70 dark:text-amber-200">
              <RotateCw aria-hidden="true" className="h-5 w-5" />
            </span>
            Apply late fines
          </CardTitle>
          <CardDescription className="sm:pl-[3.25rem]">
            Recalculates fines from scratch using active rules. Re-running is
            safe; it does not accumulate duplicate fines.
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex flex-col gap-3 sm:flex-row sm:items-end">
            <div><label htmlFor="fine-as-of-date" className="mb-1 block text-sm font-medium">As of date <span className="text-muted-foreground">(optional; defaults to today)</span></label><input id="fine-as-of-date" type="date" value={asOfDate} onChange={(event) => setAsOfDate(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2 sm:w-auto" /></div>
            <Button type="button" onClick={() => void handleApplyLateFines()} disabled={applyLateFinesMutation.isPending}>{applyLateFinesMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <RotateCw className="mr-2 h-4 w-4" />}Apply late fines</Button>
          </div>
          {lateFineResult && <div className="space-y-3 rounded-xl border bg-muted/20 p-4">
            <h3 className="flex items-center gap-2 text-sm font-semibold"><CheckCircle2 className="h-4 w-4 text-emerald-600" />Recalculation result</h3>
            <div className="grid grid-cols-2 gap-3 text-sm sm:grid-cols-3 lg:grid-cols-6">
              <div className="rounded-lg border bg-card p-3"><p className="text-xs text-muted-foreground">Evaluated</p><strong className="mt-1 block text-lg tabular-nums">{lateFineResult.totalInvoicesEvaluated}</strong></div>
              <div className="rounded-lg border bg-card p-3"><p className="text-xs text-muted-foreground">Updated</p><strong className="mt-1 block text-lg tabular-nums text-emerald-700">{lateFineResult.invoicesUpdated}</strong></div>
              <div className="rounded-lg border bg-card p-3"><p className="text-xs text-muted-foreground">Within grace</p><strong className="mt-1 block text-lg tabular-nums">{lateFineResult.skippedWithinGracePeriod}</strong></div>
              <div className="rounded-lg border bg-card p-3"><p className="text-xs text-muted-foreground">No rule</p><strong className="mt-1 block text-lg tabular-nums">{lateFineResult.skippedNoRule}</strong></div>
              <div className="rounded-lg border bg-card p-3"><p className="text-xs text-muted-foreground">Failed</p><strong className="mt-1 block text-lg tabular-nums text-rose-700">{lateFineResult.failed}</strong></div>
              <div className="rounded-lg border bg-card p-3"><p className="text-xs text-muted-foreground">Fine delta</p><strong className="mt-1 block text-sm tabular-nums">BDT {money(lateFineResult.totalFineApplied)}</strong></div>
            </div>
            {lateFineResult.errors.length > 0 && <div className="max-h-48 space-y-2 overflow-y-auto border-t pt-3">{lateFineResult.errors.map((error, index) => <p key={`${error.studentId}-${index}`} className="text-sm text-destructive">{error.studentName ? `${error.studentName}: ` : ""}{error.reason}</p>)}</div>}
          </div>}
        </CardContent>
      </Card>}

      {canView && <Card>
        <CardHeader className="border-b bg-muted/20">
          <CardTitle className="flex items-center gap-3">
            <span className="flex h-10 w-10 items-center justify-center rounded-lg bg-primary/10 text-primary">
              <Users aria-hidden="true" className="h-5 w-5" />
            </span>
            <span>Invoice register</span>
          </CardTitle>
          <CardDescription className="sm:pl-[3.25rem]">
            {invoicePage?.totalCount ?? 0} matching invoices
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 rounded-xl border bg-muted/20 p-4 sm:grid-cols-3">
            <select aria-label="Filter by invoice status" value={filters.status ?? ""} onChange={(event) => changeFilter("status", event.target.value ? Number(event.target.value) as InvoiceStatusValue : undefined)} className="w-full rounded-md border bg-background px-3 py-2 text-sm"><option value="">All statuses</option>{statuses.map((label, index) => <option key={label} value={index + 1}>{label}</option>)}</select>
            <select aria-label="Filter by student" value={filters.studentId ?? ""} onChange={(event) => changeFilter("studentId", event.target.value ? Number(event.target.value) : undefined)} className="w-full rounded-md border bg-background px-3 py-2 text-sm"><option value="">All students</option>{students.map((student) => <option key={student.id} value={student.id}>{student.fullName} · {student.admissionNumber}</option>)}</select>
            <select aria-label="Filter by academic year" value={filters.academicYearId ?? ""} onChange={(event) => changeFilter("academicYearId", event.target.value ? Number(event.target.value) : undefined)} className="w-full rounded-md border bg-background px-3 py-2 text-sm"><option value="">All academic years</option>{years.map((year) => <option key={year.id} value={year.id}>{year.name}</option>)}</select>
          </div>
          {isPending && <p className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading invoices…</p>}
          {isError && <p className="rounded-md border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load invoices.</p>}
          {!isPending && !isError && !invoicePage?.items.length && <p className="rounded-md border border-dashed p-6 text-sm text-muted-foreground">No invoices match these filters.</p>}
          {!!invoicePage?.items.length && <>
            <div className="space-y-3">{invoicePage.items.map((item) => {
              const paidAmount = Math.max(0, item.totalAmount - item.balanceDue);
              const paidPercent = item.totalAmount > 0
                ? Math.min(100, Math.max(0, (paidAmount / item.totalAmount) * 100))
                : 0;
              return <div key={item.id} className={`grid gap-4 rounded-xl border border-l-4 bg-card p-4 shadow-sm transition hover:shadow-md sm:grid-cols-[minmax(0,1.3fr)_minmax(0,0.9fr)_minmax(0,1fr)_auto] sm:items-center ${statusAccentClass(item.status)}`}>
                <div className="min-w-0">
                  <p className="truncate font-semibold">{item.invoiceNumber}</p>
                  <p className="mt-1 truncate text-sm text-muted-foreground">{item.studentName}</p>
                </div>
                <div>
                  <span className={`inline-flex rounded-full border px-2.5 py-1 text-xs font-semibold ${statusClass(item.status)}`}>{statusLabel(item.status)}</span>
                  <p className="mt-2 flex items-center gap-1.5 text-xs text-muted-foreground"><Clock3 aria-hidden="true" className="h-3.5 w-3.5" />Due {new Date(item.dueDate).toLocaleDateString()}</p>
                </div>
                <div className="grid grid-cols-2 gap-3 sm:block sm:space-y-2">
                  <div><p className="text-xs text-muted-foreground">Invoice total</p><p className="font-semibold tabular-nums">BDT {money(item.totalAmount)}</p></div>
                  <div><p className="text-xs text-muted-foreground">Balance due</p><p className={`font-semibold tabular-nums ${item.balanceDue > 0 ? "text-amber-700 dark:text-amber-300" : "text-emerald-700 dark:text-emerald-300"}`}>BDT {money(item.balanceDue)}</p></div>
                  <div className="col-span-2 h-1.5 overflow-hidden rounded-full bg-muted sm:col-span-1"><div className="h-full rounded-full bg-emerald-500 transition-[width]" style={{ width: `${paidPercent}%` }} /></div>
                </div>
                <Button type="button" variant="outline" className="w-full border-primary/20 text-primary hover:bg-primary/5 sm:w-auto" onClick={() => setSelectedId(item.id)}>
                  <FileText aria-hidden="true" className="mr-2 h-4 w-4" />
                  View details
                </Button>
              </div>;
            })}</div>
            <div className="flex items-center justify-between border-t pt-4 text-sm"><span className="text-muted-foreground">Page {invoicePage.pageNumber} of {Math.max(invoicePage.totalPages, 1)}</span><div className="flex gap-2"><Button type="button" variant="outline" className="h-9 w-9 px-0 py-0" aria-label="Previous page" disabled={!invoicePage.hasPreviousPage || isPending} onClick={() => setFilters((value) => ({ ...value, pageNumber: Math.max(1, value.pageNumber - 1) }))}><ChevronLeft className="h-4 w-4" /></Button><Button type="button" variant="outline" className="h-9 w-9 px-0 py-0" aria-label="Next page" disabled={!invoicePage.hasNextPage || isPending} onClick={() => setFilters((value) => ({ ...value, pageNumber: value.pageNumber + 1 }))}><ChevronRight className="h-4 w-4" /></Button></div></div>
          </>}
        </CardContent>
      </Card>}

      {selectedId !== null && <Card className="overflow-hidden border-primary/20">
        <CardHeader className="border-b bg-gradient-to-r from-primary/10 via-primary/5 to-transparent">
          <div className="flex items-start justify-between gap-3">
            <div className="flex items-start gap-3">
              <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary"><ReceiptText aria-hidden="true" className="h-5 w-5" /></span>
              <div><CardTitle>Invoice details</CardTitle><CardDescription className="mt-1">Invoice number and totals are assigned by the server.</CardDescription></div>
            </div>
            <Button type="button" variant="outline" className="h-9 w-9 shrink-0 px-0 py-0" aria-label="Close invoice details" onClick={() => setSelectedId(null)}><X className="h-4 w-4" /></Button>
          </div>
        </CardHeader>
        <CardContent>
          {detailPending && <p className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading invoice…</p>}
          {detailError && <p className="text-sm text-destructive">Unable to load invoice details.</p>}
          {invoice && <div className="space-y-5">
            <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
              <div className="rounded-xl border bg-muted/20 p-4"><p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">Invoice</p><p className="mt-1 font-semibold">{invoice.invoiceNumber}</p></div>
              <div className="rounded-xl border bg-muted/20 p-4"><p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">Student</p><p className="mt-1 font-semibold">{invoice.studentName}</p><p className="text-xs text-muted-foreground">{invoice.studentAdmissionNumber}</p></div>
              <div className="rounded-xl border bg-muted/20 p-4"><p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">Academic year</p><p className="mt-1 font-semibold">{invoice.academicYearName}</p></div>
              <div className="rounded-xl border bg-muted/20 p-4"><p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">Status</p><span className={`mt-1 inline-flex rounded-full border px-2.5 py-1 text-xs font-semibold ${statusClass(invoice.status)}`}>{statusLabel(invoice.status)}</span></div>
              <div className="rounded-xl border bg-muted/20 p-4"><p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">Invoice date</p><p className="mt-1 font-medium">{new Date(invoice.invoiceDate).toLocaleDateString()}</p></div>
              <div className="rounded-xl border bg-muted/20 p-4"><p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">Due date</p><p className="mt-1 font-medium">{new Date(invoice.dueDate).toLocaleDateString()}</p></div>
              <div className="rounded-xl border border-emerald-200 bg-emerald-50 p-4 dark:border-emerald-900 dark:bg-emerald-950/40"><p className="text-xs font-medium uppercase tracking-wide text-emerald-800 dark:text-emerald-300">Paid</p><p className="mt-1 font-semibold tabular-nums text-emerald-900 dark:text-emerald-100">BDT {money(invoice.amountPaid)}</p></div>
              <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 dark:border-amber-900 dark:bg-amber-950/40"><p className="text-xs font-medium uppercase tracking-wide text-amber-800 dark:text-amber-300">Balance due</p><p className="mt-1 font-semibold tabular-nums text-amber-900 dark:text-amber-100">BDT {money(invoice.balanceDue)}</p></div>
            </div>
            <div className="space-y-3 border-t pt-5">
              <h3 className="flex items-center gap-2 text-sm font-semibold"><Sparkles aria-hidden="true" className="h-4 w-4 text-primary" />Line items</h3>
              {invoice.items.map((item) => <div key={item.id} className="grid gap-3 rounded-xl border bg-card p-4 text-sm sm:grid-cols-[minmax(0,1.4fr)_repeat(4,minmax(0,1fr))] sm:items-center">
                <div><p className="font-semibold">{item.feeTypeName}</p><p className="mt-1 text-xs text-muted-foreground">{item.description}</p></div>
                <div><p className="text-xs text-muted-foreground">Original</p><p className="font-medium tabular-nums">BDT {money(item.originalAmount)}</p></div>
                <div><p className="text-xs text-muted-foreground">Discount</p><p className="font-medium tabular-nums text-emerald-700">BDT {money(item.discountAmount)}</p></div>
                <div><p className="text-xs text-muted-foreground">Fine</p><p className="font-medium tabular-nums text-rose-700">BDT {money(item.fineAmount)}</p></div>
                <div><p className="text-xs text-muted-foreground">Net × {item.quantity}</p><p className="font-semibold tabular-nums">BDT {money(item.netAmount * item.quantity)}</p></div>
              </div>)}
            </div>
            {invoice.notes && <p className="text-sm text-muted-foreground">Notes: {invoice.notes}</p>}{invoice.cancellationReason && <p className="text-sm text-destructive">Cancellation reason: {invoice.cancellationReason}</p>}
            {canManagePayments && <Link to={`/fees/payments?invoiceId=${invoice.id}`} className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">Payments</Link>}
            {canCancel && invoice.amountPaid === 0 && (invoice.status === InvoiceStatus.Issued || invoice.status === InvoiceStatus.Overdue) && <Button type="button" variant="outline" disabled={cancelMutation.isPending} onClick={() => void cancelSelectedInvoice()}>{cancelMutation.isPending && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}Cancel invoice</Button>}
            {canCancel && invoice.amountPaid > 0 && <p className="text-sm text-muted-foreground">This invoice has payments and cannot be cancelled. Void payments first.</p>}
          </div>}
        </CardContent>
      </Card>}
    </div>
  );
}