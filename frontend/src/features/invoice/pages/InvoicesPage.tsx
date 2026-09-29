import { useMemo, useState } from "react";
import type { AxiosError } from "axios";
import { Link } from "react-router-dom";
import { useQueryClient } from "@tanstack/react-query";
import { ChevronLeft, ChevronRight, FileText, Loader2, Plus, X } from "lucide-react";
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
import { useCancelInvoice, useCreateInvoice, useGenerateMonthlyInvoices, useInvoice, useInvoices } from "../hooks/useInvoiceData";
import { InvoiceStatus, type InvoiceFilters, type InvoiceGenerationResultDto, type InvoiceStatusValue } from "../types/invoice.types";

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
  if (value === InvoiceStatus.Paid) return "bg-primary/10 text-primary";
  if (value === InvoiceStatus.Overdue) return "bg-destructive/10 text-destructive";
  if (value === InvoiceStatus.Cancelled) return "bg-muted text-muted-foreground";
  if (value === InvoiceStatus.PartiallyPaid) return "bg-amber-500/10 text-amber-700";
  return "bg-secondary text-secondary-foreground";
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
  const canViewConcessions = hasPermission(Permission.ConcessionView);
  const canManagePayments = hasPermission(Permission.PaymentView) || hasPermission(Permission.PaymentCollect);
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

  const [generationYear, setGenerationYear] = useState("");
  const [generationMonth, setGenerationMonth] = useState(String(new Date().getMonth() + 1));
  const [generationCalendarYear, setGenerationCalendarYear] = useState(String(new Date().getFullYear()));
  const [generationClass, setGenerationClass] = useState("");
  const [generationInvoiceDate, setGenerationInvoiceDate] = useState(localDate(new Date()));
  const [generationDueDate, setGenerationDueDate] = useState(datePlus(7));
  const [generationResult, setGenerationResult] = useState<InvoiceGenerationResultDto | null>(null);

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
      <header><h1 className="font-display text-2xl font-semibold">Invoices</h1><p className="text-sm text-muted-foreground">Create student bills, follow balances, and review invoice history.</p></header>

      {canCreate && <div className="grid gap-6 xl:grid-cols-2">
        <Card>
          <CardHeader><CardTitle>Create invoice</CardTitle><CardDescription>Header totals are assigned by the server. Eligible concessions are applied from student records.</CardDescription></CardHeader>
          <CardContent>
            {!canViewConcessions ? <p className="rounded-md border border-amber-500/30 bg-amber-500/5 p-4 text-sm">Concession view permission is required to verify and apply student discounts.</p> : <form onSubmit={(event) => void submitInvoice(event)} className="space-y-4">
              <div className="grid gap-3 sm:grid-cols-2">
                <div><label className="mb-1 block text-sm font-medium">Student</label><select required value={studentId} onChange={(event) => { setStudentId(event.target.value); setStructureId(""); setItems([]); }} className="w-full rounded-md border bg-background px-3 py-2"><option value="">Select student</option>{students.map((student) => <option key={student.id} value={student.id}>{student.fullName} · {student.admissionNumber}</option>)}</select></div>
                <div><label className="mb-1 block text-sm font-medium">Academic year</label><select required value={yearId} onChange={(event) => { setYearId(event.target.value); setStructureId(""); setItems([]); }} className="w-full rounded-md border bg-background px-3 py-2"><option value="">Select year</option>{years.map((year) => <option key={year.id} value={year.id}>{year.name}</option>)}</select></div>
                <div><label className="mb-1 block text-sm font-medium">Fee structure <span className="text-muted-foreground">(optional)</span></label><select disabled={!currentStudent || !yearId} value={structureId} onChange={(event) => void selectStructure(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2"><option value="">Add items manually</option>{structures.filter((item) => !item.sectionId || item.sectionId === currentStudent?.sectionId).map((item) => <option key={item.id} value={item.id}>{item.name}{item.sectionName ? ` · ${item.sectionName}` : " · All sections"}</option>)}</select></div>
                <div className="grid grid-cols-2 gap-2"><div><label className="mb-1 block text-sm font-medium">Invoice date</label><input type="date" required value={invoiceDate} onChange={(event) => setInvoiceDate(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div><div><label className="mb-1 block text-sm font-medium">Due date</label><input type="date" min={invoiceDate} required value={dueDate} onChange={(event) => setDueDate(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div></div>
                <div className="grid grid-cols-2 gap-2"><div><label className="mb-1 block text-sm font-medium">Billing month</label><select value={billingMonth} onChange={(event) => setBillingMonth(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2"><option value="">Not monthly</option>{Array.from({ length: 12 }, (_, index) => <option key={index + 1} value={index + 1}>{new Date(2000, index).toLocaleString(undefined, { month: "long" })}</option>)}</select></div><div><label className="mb-1 block text-sm font-medium">Billing year</label><input type="number" min="2000" max="2100" disabled={!billingMonth} value={billingYear} onChange={(event) => setBillingYear(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div></div>
              </div>

              {studentId && yearId && concessionsError && <p className="text-sm text-destructive">Concessions could not be verified. Invoice creation is disabled until the lookup succeeds.</p>}
              <div className="space-y-3 border-t pt-4">
                <div className="flex items-center justify-between"><div><h3 className="text-sm font-semibold">Invoice items</h3><p className="text-xs text-muted-foreground">Discounts are read-only and calculated from active, approved concessions.</p></div><Button type="button" variant="outline" className="px-3 py-1.5" disabled={!feeTypes.length} onClick={() => addItem()}><Plus className="mr-2 h-4 w-4" />Add item</Button></div>
                {loadingStructure && <p className="flex items-center text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading fee structure…</p>}
                {items.map((item, index) => {
                  const computed = computedItems[index];
                  return <div key={`${item.sortOrder}-${index}`} className="grid gap-3 rounded-md border p-3 sm:grid-cols-[1.4fr_1fr_auto] sm:items-end">
                    <div><label className="mb-1 block text-xs font-medium">Fee type</label><select required value={item.feeTypeId} onChange={(event) => { const feeType = feeTypes.find((type) => String(type.id) === event.target.value); setItems((current) => current.map((value, itemIndex) => itemIndex === index ? { ...value, feeTypeId: event.target.value, description: feeType?.name ?? "" } : value)); }} className="w-full rounded-md border bg-background px-3 py-2 text-sm"><option value="">Select fee type</option>{feeTypes.filter((type) => type.isActive || type.id === Number(item.feeTypeId)).map((type) => <option key={type.id} value={type.id}>{type.name} · {type.code}</option>)}</select></div>
                    <div className="grid grid-cols-2 gap-2"><div><label className="mb-1 block text-xs font-medium">Amount</label><input type="number" required min="0.01" step="0.01" value={item.originalAmount} onChange={(event) => setItems((current) => current.map((value, itemIndex) => itemIndex === index ? { ...value, originalAmount: event.target.value } : value))} className="w-full rounded-md border bg-background px-3 py-2 text-sm" /></div><div><label className="mb-1 block text-xs font-medium">Qty</label><input type="number" required min="1" step="1" value={item.quantity} onChange={(event) => setItems((current) => current.map((value, itemIndex) => itemIndex === index ? { ...value, quantity: event.target.value } : value))} className="w-full rounded-md border bg-background px-3 py-2 text-sm" /></div><div><label className="mb-1 block text-xs font-medium">Fine</label><input type="number" min="0" step="0.01" value={item.fineAmount} onChange={(event) => setItems((current) => current.map((value, itemIndex) => itemIndex === index ? { ...value, fineAmount: event.target.value } : value))} className="w-full rounded-md border bg-background px-3 py-2 text-sm" /></div><p className="self-end pb-2 text-xs text-muted-foreground">Discount {money(computed.discountAmount)}</p></div>
                    <Button type="button" variant="outline" className="h-9 w-9 px-0 py-0" aria-label="Remove invoice item" onClick={() => setItems((current) => current.filter((_, itemIndex) => itemIndex !== index))}><X className="h-4 w-4" /></Button>
                    <div className="sm:col-span-3"><label className="mb-1 block text-xs font-medium">Description</label><input required maxLength={200} value={item.description} onChange={(event) => setItems((current) => current.map((value, itemIndex) => itemIndex === index ? { ...value, description: event.target.value } : value))} className="w-full rounded-md border bg-background px-3 py-2 text-sm" /></div>
                  </div>;
                })}
                {!items.length && <p className="rounded-md border border-dashed p-4 text-sm text-muted-foreground">Select a fee structure or add an invoice item.</p>}
                <div className="flex justify-end gap-4 border-t pt-3 text-sm"><span className="text-muted-foreground">Invoice total</span><strong>BDT {money(total)}</strong></div>
              </div>
              <div><label className="mb-1 block text-sm font-medium">Notes</label><textarea maxLength={500} rows={2} value={notes} onChange={(event) => setNotes(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div>
              <Button type="submit" disabled={createMutation.isPending || loadingStructure || concessionsPending || concessionsError || !items.length}>{createMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <FileText className="mr-2 h-4 w-4" />}Create invoice</Button>
            </form>}
          </CardContent>
        </Card>

        <Card>
          <CardHeader><CardTitle>Generate monthly invoices</CardTitle><CardDescription>Existing invoices are skipped; review partial failures returned by the server.</CardDescription></CardHeader>
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
          {generationResult && <div className="mt-5 space-y-3 rounded-md border p-4"><h3 className="text-sm font-semibold">Generation result</h3><div className="grid grid-cols-2 gap-3 text-sm sm:grid-cols-3"><p>Evaluated <strong>{generationResult.totalStudentsEvaluated}</strong></p><p>Created <strong>{generationResult.invoicesCreated}</strong></p><p>Already invoiced <strong>{generationResult.skippedAlreadyInvoiced}</strong></p><p>No monthly items <strong>{generationResult.skippedNoMonthlyItems}</strong></p><p>Failed <strong>{generationResult.failed}</strong></p></div>{generationResult.errors.map((error) => <p key={`${error.studentId}-${error.reason}`} className="text-sm text-destructive">{error.studentName}: {error.reason}</p>)}</div>}
          </CardContent>
        </Card>
      </div>}

      {canView && <Card>
        <CardHeader><CardTitle>Invoice register</CardTitle><CardDescription>{invoicePage?.totalCount ?? 0} matching invoices</CardDescription></CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 sm:grid-cols-3">
            <select aria-label="Filter by invoice status" value={filters.status ?? ""} onChange={(event) => changeFilter("status", event.target.value ? Number(event.target.value) as InvoiceStatusValue : undefined)} className="w-full rounded-md border bg-background px-3 py-2 text-sm"><option value="">All statuses</option>{statuses.map((label, index) => <option key={label} value={index + 1}>{label}</option>)}</select>
            <select aria-label="Filter by student" value={filters.studentId ?? ""} onChange={(event) => changeFilter("studentId", event.target.value ? Number(event.target.value) : undefined)} className="w-full rounded-md border bg-background px-3 py-2 text-sm"><option value="">All students</option>{students.map((student) => <option key={student.id} value={student.id}>{student.fullName} · {student.admissionNumber}</option>)}</select>
            <select aria-label="Filter by academic year" value={filters.academicYearId ?? ""} onChange={(event) => changeFilter("academicYearId", event.target.value ? Number(event.target.value) : undefined)} className="w-full rounded-md border bg-background px-3 py-2 text-sm"><option value="">All academic years</option>{years.map((year) => <option key={year.id} value={year.id}>{year.name}</option>)}</select>
          </div>
          {isPending && <p className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading invoices…</p>}
          {isError && <p className="rounded-md border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load invoices.</p>}
          {!isPending && !isError && !invoicePage?.items.length && <p className="rounded-md border border-dashed p-6 text-sm text-muted-foreground">No invoices match these filters.</p>}
          {!!invoicePage?.items.length && <>
            <div className="space-y-2">{invoicePage.items.map((item) => <div key={item.id} className="grid gap-3 rounded-md border p-4 sm:grid-cols-[1.2fr_1fr_auto_auto_auto] sm:items-center"><div><p className="font-medium">{item.invoiceNumber}</p><p className="text-sm text-muted-foreground">{item.studentName}</p></div><div><span className={`rounded-full px-2 py-1 text-xs font-medium ${statusClass(item.status)}`}>{statusLabel(item.status)}</span><p className="mt-1 text-xs text-muted-foreground">Due {new Date(item.dueDate).toLocaleDateString()}</p></div><p className="text-sm">Total <strong>BDT {money(item.totalAmount)}</strong></p><p className="text-sm">Balance <strong>BDT {money(item.balanceDue)}</strong></p><Button type="button" variant="outline" className="px-3 py-1.5" onClick={() => setSelectedId(item.id)}>Details</Button></div>)}</div>
            <div className="flex items-center justify-between border-t pt-4 text-sm"><span className="text-muted-foreground">Page {invoicePage.pageNumber} of {Math.max(invoicePage.totalPages, 1)}</span><div className="flex gap-2"><Button type="button" variant="outline" className="h-9 w-9 px-0 py-0" aria-label="Previous page" disabled={!invoicePage.hasPreviousPage || isPending} onClick={() => setFilters((value) => ({ ...value, pageNumber: Math.max(1, value.pageNumber - 1) }))}><ChevronLeft className="h-4 w-4" /></Button><Button type="button" variant="outline" className="h-9 w-9 px-0 py-0" aria-label="Next page" disabled={!invoicePage.hasNextPage || isPending} onClick={() => setFilters((value) => ({ ...value, pageNumber: value.pageNumber + 1 }))}><ChevronRight className="h-4 w-4" /></Button></div></div>
          </>}
        </CardContent>
      </Card>}

      {selectedId !== null && <Card>
        <CardHeader><div className="flex items-start justify-between gap-3"><div><CardTitle>Invoice details</CardTitle><CardDescription>Invoice number and totals are assigned by the server.</CardDescription></div><Button type="button" variant="outline" className="h-9 w-9 px-0 py-0" aria-label="Close invoice details" onClick={() => setSelectedId(null)}><X className="h-4 w-4" /></Button></div></CardHeader>
        <CardContent>
          {detailPending && <p className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading invoice…</p>}
          {detailError && <p className="text-sm text-destructive">Unable to load invoice details.</p>}
          {invoice && <div className="space-y-5">
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4"><div><p className="text-xs text-muted-foreground">Invoice</p><p className="font-medium">{invoice.invoiceNumber}</p></div><div><p className="text-xs text-muted-foreground">Student</p><p className="font-medium">{invoice.studentName} · {invoice.studentAdmissionNumber}</p></div><div><p className="text-xs text-muted-foreground">Academic year</p><p>{invoice.academicYearName}</p></div><div><p className="text-xs text-muted-foreground">Status</p><span className={`inline-block rounded-full px-2 py-1 text-xs font-medium ${statusClass(invoice.status)}`}>{statusLabel(invoice.status)}</span></div><div><p className="text-xs text-muted-foreground">Invoice date</p><p>{new Date(invoice.invoiceDate).toLocaleDateString()}</p></div><div><p className="text-xs text-muted-foreground">Due date</p><p>{new Date(invoice.dueDate).toLocaleDateString()}</p></div><div><p className="text-xs text-muted-foreground">Paid</p><p>BDT {money(invoice.amountPaid)}</p></div><div><p className="text-xs text-muted-foreground">Balance due</p><p className="font-semibold">BDT {money(invoice.balanceDue)}</p></div></div>
            <div className="space-y-2 border-t pt-4"><h3 className="text-sm font-semibold">Line items</h3>{invoice.items.map((item) => <div key={item.id} className="grid gap-2 rounded-md border p-3 text-sm sm:grid-cols-[1.4fr_repeat(4,1fr)]"><div><p className="font-medium">{item.feeTypeName}</p><p className="text-xs text-muted-foreground">{item.description}</p></div><p>Original {money(item.originalAmount)}</p><p>Discount {money(item.discountAmount)}</p><p>Fine {money(item.fineAmount)}</p><p>Net × {item.quantity}: {money(item.netAmount * item.quantity)}</p></div>)}</div>
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