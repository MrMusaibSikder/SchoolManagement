import { useState } from "react";
import type { AxiosError } from "axios";
import { Link, useSearchParams } from "react-router-dom";
import { Check, ChevronLeft, ChevronRight, Loader2, ShieldAlert, X } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useAcademicYears } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { useInvoice, useInvoices } from "@/features/invoice/hooks/useInvoiceData";
import type { InvoiceFilters } from "@/features/invoice/types/invoice.types";
import { useStudents } from "@/features/student/hooks/useStudentData";
import { Permission } from "@/lib/permissions";
import { useCollectPayment, useInvoicePayments, usePayment, useVoidPayment } from "../hooks/usePaymentData";
import { PaymentMethod, PaymentStatus, type PaymentMethodValue } from "../types/payment.types";

const PAGE_SIZE = 20;
const MAX_PAYMENT_DATE = new Date(Date.now() + 86_400_000).toISOString().slice(0, 10);
const methods = [
  { value: PaymentMethod.Cash, label: "Cash" },
  { value: PaymentMethod.BankTransfer, label: "Bank transfer" },
  { value: PaymentMethod.Card, label: "Card" },
  { value: PaymentMethod.MobileBanking, label: "Mobile banking" },
  { value: PaymentMethod.Cheque, label: "Cheque" },
] as const;

function money(amount: number) {
  return new Intl.NumberFormat("en-BD", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(amount);
}

function errorMessage(error: unknown, fallback: string) {
  const response = (error as AxiosError<{ message?: string } | string>).response;
  if (typeof response?.data === "string" && response.data) return response.data;
  if (response?.data && typeof response.data === "object" && response.data.message) return response.data.message;
  if (response?.status === 409) return "This invoice changed during the payment action. The latest balance and history have been refreshed.";
  return fallback;
}

function methodLabel(method: number) {
  return methods.find((item) => item.value === method)?.label ?? "Unknown";
}

export function PaymentsPage() {
  const { hasPermission } = usePermissions();
  const canViewPayments = hasPermission(Permission.PaymentView);
  const canCollect = hasPermission(Permission.PaymentCollect);
  const canVoid = hasPermission(Permission.PaymentVoid);
  const canViewInvoices = hasPermission(Permission.InvoiceView);
  const [searchParams, setSearchParams] = useSearchParams();
  const [filters, setFilters] = useState<InvoiceFilters>({ pageNumber: 1, pageSize: PAGE_SIZE });
  const { data: students = [] } = useStudents();
  const { data: academicYears = [] } = useAcademicYears();
  const { data: invoices, isPending: invoicesPending, isError: invoicesError } = useInvoices(filters, canViewInvoices);
  const selectedInvoiceId = searchParams.get("invoiceId") ? Number(searchParams.get("invoiceId")) : null;
  const { data: invoice, isPending: invoicePending, isError: invoiceError } = useInvoice(selectedInvoiceId, canViewInvoices);
  const { data: payments = [], isPending: paymentsPending, isError: paymentsError } = useInvoicePayments(selectedInvoiceId, canViewPayments);
  const [selectedPaymentId, setSelectedPaymentId] = useState<number | null>(null);
  const { data: paymentDetails, isPending: paymentDetailsPending } = usePayment(selectedPaymentId);

  const [amount, setAmount] = useState("");
  const [method, setMethod] = useState<PaymentMethodValue>(PaymentMethod.Cash);
  const [paymentDate, setPaymentDate] = useState(new Date().toISOString().slice(0, 10));
  const [transactionId, setTransactionId] = useState("");
  const [remarks, setRemarks] = useState("");
  const [voidReason, setVoidReason] = useState("");
  const [voidingPaymentId, setVoidingPaymentId] = useState<number | null>(null);
  const collectMutation = useCollectPayment();
  const voidMutation = useVoidPayment();

  function updateFilter<K extends keyof InvoiceFilters>(key: K, value: InvoiceFilters[K] | undefined) {
    setFilters((current) => ({ ...current, [key]: value, pageNumber: 1 }));
  }

  async function submitPayment(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!invoice || !selectedInvoiceId) return;
    const numericAmount = Number(amount);
    if (!Number.isFinite(numericAmount) || numericAmount <= 0) return toast.error("Payment amount must be greater than zero.");
    if (numericAmount > invoice.balanceDue) return toast.error(`Payment cannot exceed the current balance of BDT ${money(invoice.balanceDue)}.`);
    if (method !== PaymentMethod.Cash && !transactionId.trim()) return toast.error("Transaction ID is required for this payment method.");
    if (transactionId.trim().length > 100) return toast.error("Transaction ID must be 100 characters or fewer.");
    if (remarks.length > 500) return toast.error("Remarks must be 500 characters or fewer.");
    try {
      const created = await collectMutation.mutateAsync({
        invoiceId: invoice.id,
        studentId: invoice.studentId,
        amount: numericAmount,
        paymentDate,
        method,
        transactionId: method === PaymentMethod.Cash ? null : transactionId.trim(),
        remarks: remarks.trim() || null,
      });
      toast.success(`Payment ${created.paymentNumber} collected. Receipt ${created.receiptNo ?? "created"}.`);
      setAmount("");
      setTransactionId("");
      setRemarks("");
      setSelectedPaymentId(created.id);
    } catch (error) {
      toast.error(errorMessage(error, "Unable to collect payment."));
    }
  }

  async function submitVoid(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!voidingPaymentId || !voidReason.trim()) return toast.error("A void reason is required.");
    if (voidReason.trim().length > 300) return toast.error("Void reason must be 300 characters or fewer.");
    try {
      await voidMutation.mutateAsync({ id: voidingPaymentId, payload: { reason: voidReason.trim() } });
      toast.success("Payment voided and invoice balance restored.");
      setVoidingPaymentId(null);
      setVoidReason("");
    } catch (error) {
      toast.error(errorMessage(error, "Unable to void payment."));
    }
  }

  if (!canViewPayments && !canCollect && !canVoid) {
    return <Card className="mx-auto max-w-4xl"><CardContent className="p-6 text-sm text-destructive">You do not have permission to access payments.</CardContent></Card>;
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
        <div><h1 className="font-display text-2xl font-semibold">Payments</h1><p className="text-sm text-muted-foreground">Collect payments against outstanding invoices and review transaction history.</p></div>
        <Link to="/fees/invoices" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">Invoices</Link>
      </div>

      {!canViewInvoices && <p className="rounded-md border border-amber-500/30 bg-amber-500/5 p-4 text-sm">Invoice view permission is required to select an invoice and verify its current balance.</p>}

      {canViewInvoices && <Card>
        <CardHeader><CardTitle>Select invoice</CardTitle><CardDescription>Only the selected invoice’s current detail can authorize a payment collection.</CardDescription></CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 sm:grid-cols-3">
            <select aria-label="Filter invoices by student" value={filters.studentId ?? ""} onChange={(event) => updateFilter("studentId", event.target.value ? Number(event.target.value) : undefined)} className="w-full rounded-md border bg-background px-3 py-2 text-sm"><option value="">All students</option>{students.map((student) => <option key={student.id} value={student.id}>{student.fullName} · {student.admissionNumber}</option>)}</select>
            <select aria-label="Filter invoices by academic year" value={filters.academicYearId ?? ""} onChange={(event) => updateFilter("academicYearId", event.target.value ? Number(event.target.value) : undefined)} className="w-full rounded-md border bg-background px-3 py-2 text-sm"><option value="">All academic years</option>{academicYears.map((year) => <option key={year.id} value={year.id}>{year.name}</option>)}</select>
            <select aria-label="Filter invoices by status" value={filters.status ?? ""} onChange={(event) => updateFilter("status", event.target.value ? Number(event.target.value) as NonNullable<InvoiceFilters["status"]> : undefined)} className="w-full rounded-md border bg-background px-3 py-2 text-sm"><option value="">All invoice statuses</option><option value="2">Issued</option><option value="3">Partially paid</option><option value="4">Paid</option><option value="5">Overdue</option><option value="6">Cancelled</option></select>
          </div>
          {invoicesPending && <p className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading invoices…</p>}
          {invoicesError && <p className="rounded-md border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load invoices.</p>}
          {invoices?.items.length ? <>
            <div className="space-y-2">{invoices.items.map((item) => <button key={item.id} type="button" onClick={() => { setSearchParams({ invoiceId: String(item.id) }); setSelectedPaymentId(null); }} className={`grid w-full gap-3 rounded-md border p-4 text-left transition hover:bg-accent/40 sm:grid-cols-[1.2fr_1fr_auto_auto] sm:items-center ${selectedInvoiceId === item.id ? "border-primary bg-primary/5" : ""}`}><span><span className="block font-medium">{item.invoiceNumber}</span><span className="text-sm text-muted-foreground">{item.studentName}</span></span><span className="text-sm">Due {new Date(item.dueDate).toLocaleDateString()}</span><span className="text-sm">Total BDT {money(item.totalAmount)}</span><span className="text-sm font-semibold">Balance BDT {money(item.balanceDue)}</span></button>)}</div>
            <div className="flex items-center justify-between border-t pt-4 text-sm"><span className="text-muted-foreground">Page {invoices.pageNumber} of {Math.max(invoices.totalPages, 1)}</span><div className="flex gap-2"><Button type="button" variant="outline" className="h-9 w-9 px-0 py-0" aria-label="Previous invoices" disabled={!invoices.hasPreviousPage || invoicesPending} onClick={() => setFilters((current) => ({ ...current, pageNumber: Math.max(1, current.pageNumber - 1) }))}><ChevronLeft className="h-4 w-4" /></Button><Button type="button" variant="outline" className="h-9 w-9 px-0 py-0" aria-label="Next invoices" disabled={!invoices.hasNextPage || invoicesPending} onClick={() => setFilters((current) => ({ ...current, pageNumber: current.pageNumber + 1 }))}><ChevronRight className="h-4 w-4" /></Button></div></div>
          </> : !invoicesPending && !invoicesError ? <p className="rounded-md border border-dashed p-6 text-sm text-muted-foreground">No invoices match these filters.</p> : null}
        </CardContent>
      </Card>}

      {selectedInvoiceId !== null && canViewInvoices && <Card>
        <CardHeader><CardTitle>Invoice payment</CardTitle><CardDescription>Balance is refreshed after every collection or void.</CardDescription></CardHeader>
        <CardContent>
          {invoicePending && <p className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading invoice balance…</p>}
          {invoiceError && <p className="text-sm text-destructive">Unable to load invoice details.</p>}
          {invoice && <div className="space-y-6">
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4"><div><p className="text-xs text-muted-foreground">Invoice</p><p className="font-medium">{invoice.invoiceNumber}</p></div><div><p className="text-xs text-muted-foreground">Student</p><p className="font-medium">{invoice.studentName}</p></div><div><p className="text-xs text-muted-foreground">Invoice status</p><p>{invoice.status}</p></div><div><p className="text-xs text-muted-foreground">Current balance due</p><p className="text-lg font-semibold">BDT {money(invoice.balanceDue)}</p></div></div>

            {canCollect && invoice.balanceDue > 0 && invoice.status !== 6 && <form onSubmit={(event) => void submitPayment(event)} className="space-y-4 rounded-md border p-4">
              <h3 className="font-semibold">Collect payment</h3>
              <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
                <div><label className="mb-1 block text-sm font-medium">Amount (BDT)</label><input required type="number" min="0.01" max={invoice.balanceDue} step="0.01" value={amount} onChange={(event) => setAmount(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /><p className="mt-1 text-xs text-muted-foreground">Maximum BDT {money(invoice.balanceDue)}</p></div>
                <div><label className="mb-1 block text-sm font-medium">Payment method</label><select value={method} onChange={(event) => setMethod(Number(event.target.value) as PaymentMethodValue)} className="w-full rounded-md border bg-background px-3 py-2">{methods.map((item) => <option key={item.value} value={item.value}>{item.label}</option>)}</select></div>
                <div><label className="mb-1 block text-sm font-medium">Payment date</label><input required type="date" max={MAX_PAYMENT_DATE} value={paymentDate} onChange={(event) => setPaymentDate(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div>
                {method !== PaymentMethod.Cash && <div><label className="mb-1 block text-sm font-medium">Transaction ID</label><input required maxLength={100} value={transactionId} onChange={(event) => setTransactionId(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div>}
                <div className="sm:col-span-2"><label className="mb-1 block text-sm font-medium">Remarks <span className="text-muted-foreground">(optional)</span></label><input maxLength={500} value={remarks} onChange={(event) => setRemarks(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div>
              </div>
              <Button type="submit" disabled={collectMutation.isPending || !amount || Number(amount) > invoice.balanceDue}>{collectMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Check className="mr-2 h-4 w-4" />}Collect payment</Button>
              <p className="text-xs text-muted-foreground">A receipt is created automatically after successful collection.</p>
            </form>}
            {invoice.balanceDue <= 0 && <p className="rounded-md border border-primary/20 bg-primary/5 p-4 text-sm">Invoice is fully paid. No additional payment can be collected.</p>}

            {canViewPayments && <div className="space-y-3 border-t pt-5">
              <h3 className="font-semibold">Payment history</h3>
              {paymentsPending && <p className="flex items-center text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading payments…</p>}
              {paymentsError && <p className="text-sm text-destructive">Unable to load payment history.</p>}
              {!paymentsPending && !paymentsError && payments.length === 0 && <p className="rounded-md border border-dashed p-4 text-sm text-muted-foreground">No payments recorded for this invoice.</p>}
              {payments.map((payment) => <div key={payment.id} className={`flex flex-col gap-3 rounded-md border p-4 sm:flex-row sm:items-center sm:justify-between ${payment.status === PaymentStatus.Voided ? "border-destructive/30 bg-destructive/5" : ""}`}>
                <div className="min-w-0"><div className="flex flex-wrap items-center gap-2"><p className={`font-medium ${payment.status === PaymentStatus.Voided ? "text-destructive line-through" : ""}`}>{payment.paymentNumber}</p><span className={`rounded-full px-2 py-0.5 text-xs font-medium ${payment.status === PaymentStatus.Voided ? "bg-destructive/10 text-destructive" : "bg-primary/10 text-primary"}`}>{payment.status === PaymentStatus.Voided ? "Voided" : payment.status === PaymentStatus.Completed ? "Completed" : payment.status === PaymentStatus.Refunded ? "Refunded" : "Failed"}</span></div><p className="text-sm text-muted-foreground">{methodLabel(payment.method)} · {new Date(payment.paymentDate).toLocaleDateString()}{payment.receiptNo ? ` · Receipt ${payment.receiptNo}` : ""}</p></div>
                <div className="flex items-center gap-3"><strong className={payment.status === PaymentStatus.Voided ? "text-destructive line-through" : ""}>BDT {money(payment.amount)}</strong><Button type="button" variant="outline" className="px-3 py-1.5" onClick={() => setSelectedPaymentId(payment.id)}>Details</Button>{canVoid && payment.status === PaymentStatus.Completed && <Button type="button" variant="outline" className="px-3 py-1.5 text-destructive" onClick={() => { setVoidingPaymentId(payment.id); setVoidReason(""); }}>Void</Button>}</div>
                {voidingPaymentId === payment.id && <form onSubmit={(event) => void submitVoid(event)} className="grid gap-2 sm:grid-cols-[1fr_auto_auto]"><div><label className="mb-1 block text-xs font-medium">Reason for voiding</label><input required maxLength={300} value={voidReason} onChange={(event) => setVoidReason(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2 text-sm" /></div><Button type="submit" variant="outline" className="self-end" disabled={voidMutation.isPending}>{voidMutation.isPending ? <Loader2 className="h-4 w-4 animate-spin" /> : "Confirm void"}</Button><Button type="button" variant="outline" className="self-end" onClick={() => setVoidingPaymentId(null)}>Cancel</Button></form>}
              </div>)}
            </div>}
          </div>}
        </CardContent>
      </Card>}

      {selectedPaymentId !== null && <Card>
        <CardHeader><div className="flex items-start justify-between gap-3"><div><CardTitle>Payment details</CardTitle><CardDescription>Collection and receipt references.</CardDescription></div><Button type="button" variant="outline" className="h-9 w-9 px-0 py-0" aria-label="Close payment details" onClick={() => setSelectedPaymentId(null)}><X className="h-4 w-4" /></Button></div></CardHeader>
        <CardContent>
          {paymentDetailsPending && <p className="flex items-center py-6 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading payment details…</p>}
          {paymentDetails && <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            <div><p className="text-xs text-muted-foreground">Payment number</p><p className={paymentDetails.status === PaymentStatus.Voided ? "font-medium text-destructive line-through" : "font-medium"}>{paymentDetails.paymentNumber}</p></div>
            <div><p className="text-xs text-muted-foreground">Invoice</p><p>{paymentDetails.invoiceNumber}</p></div>
            <div><p className="text-xs text-muted-foreground">Status</p><p className={paymentDetails.status === PaymentStatus.Voided ? "font-medium text-destructive" : "font-medium"}>{paymentDetails.status === PaymentStatus.Voided ? "Voided" : "Completed"}</p></div>
            <div><p className="text-xs text-muted-foreground">Amount</p><p>BDT {money(paymentDetails.amount)}</p></div>
            <div><p className="text-xs text-muted-foreground">Method</p><p>{methodLabel(paymentDetails.method)}</p></div>
            <div><p className="text-xs text-muted-foreground">Collected by</p><p>{paymentDetails.collectedByEmployeeName}</p></div>
            <div><p className="text-xs text-muted-foreground">Transaction ID</p><p>{paymentDetails.transactionId || "Not provided"}</p></div>
            <div><p className="text-xs text-muted-foreground">Receipt</p><p>{paymentDetails.receiptNo || "Not available"}</p></div>
            <div><p className="text-xs text-muted-foreground">Payment date</p><p>{new Date(paymentDetails.paymentDate).toLocaleDateString()}</p></div>
            {paymentDetails.remarks && <p className="text-sm text-muted-foreground sm:col-span-2 lg:col-span-3">Remarks: {paymentDetails.remarks}</p>}
          </div>}
        </CardContent>
      </Card>}
      {!canViewPayments && canCollect && <p className="flex items-center gap-2 text-xs text-muted-foreground"><ShieldAlert className="h-4 w-4" />Payment history is hidden because you do not have payment view permission.</p>}
    </div>
  );
}