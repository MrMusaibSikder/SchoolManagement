import { useState } from "react";
import type { AxiosError } from "axios";
import { Link, useSearchParams } from "react-router-dom";
import { AlertTriangle, Download, Loader2, Search, X } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { useDownloadReceiptPdf, useReceiptById, useReceiptByPaymentId, useVoidReceipt } from "../hooks/useReceiptData";

function apiErrorMessage(error: unknown, fallback: string) {
  const response = (error as AxiosError<{ message?: string } | string>).response;
  if (typeof response?.data === "string" && response.data) return response.data;
  if (response?.data && typeof response.data === "object" && response.data.message) return response.data.message;
  return fallback;
}

export function ReceiptsPage() {
  const { hasPermission } = usePermissions();
  const canView = hasPermission(Permission.ReceiptView);
  const canVoid = hasPermission(Permission.ReceiptVoid);
  const [searchParams, setSearchParams] = useSearchParams();
  const receiptIdParam = searchParams.get("receiptId");
  const paymentIdParam = searchParams.get("paymentId");
  const receiptId = receiptIdParam && Number(receiptIdParam) > 0 ? Number(receiptIdParam) : null;
  const paymentId = !receiptId ? paymentIdParam && Number(paymentIdParam) > 0 ? Number(paymentIdParam) : null : null;

  const [receiptIdInput, setReceiptIdInput] = useState(receiptIdParam ?? "");
  const [paymentIdInput, setPaymentIdInput] = useState(paymentIdParam ?? "");
  const [voidReason, setVoidReason] = useState("");
  const [showVoidForm, setShowVoidForm] = useState(false);
  const receiptById = useReceiptById(receiptId);
  const receiptByPayment = useReceiptByPaymentId(paymentId);
  const receipt = receiptId ? receiptById.data : receiptByPayment.data;
  const isPending = receiptId ? receiptById.isPending : receiptByPayment.isPending;
  const isError = receiptId ? receiptById.isError : receiptByPayment.isError;
  const downloadMutation = useDownloadReceiptPdf();
  const voidMutation = useVoidReceipt();

  function findByReceiptId(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const id = Number(receiptIdInput);
    if (!Number.isInteger(id) || id <= 0) return toast.error("Enter a valid receipt ID.");
    setPaymentIdInput("");
    setVoidReason("");
    setShowVoidForm(false);
    setSearchParams({ receiptId: String(id) });
  }

  function findByPaymentId(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const id = Number(paymentIdInput);
    if (!Number.isInteger(id) || id <= 0) return toast.error("Enter a valid payment ID.");
    setReceiptIdInput("");
    setVoidReason("");
    setShowVoidForm(false);
    setSearchParams({ paymentId: String(id) });
  }

  async function downloadPdf() {
    if (!receipt) return;
    try {
      const blob = await downloadMutation.mutateAsync(receipt.id);
      const url = URL.createObjectURL(blob);
      const anchor = document.createElement("a");
      anchor.href = url;
      anchor.download = `Receipt-${receipt.id}.pdf`;
      document.body.appendChild(anchor);
      anchor.click();
      anchor.remove();
      URL.revokeObjectURL(url);
    } catch (error) {
      toast.error(apiErrorMessage(error, "Unable to download receipt PDF."));
    }
  }

  async function submitVoid(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!receipt || !voidReason.trim()) return toast.error("A void reason is required.");
    if (voidReason.trim().length > 300) return toast.error("Void reason must be 300 characters or fewer.");
    if (!window.confirm("This only voids the receipt record. It does not reverse the payment or change the invoice balance. Continue?")) return;
    try {
      await voidMutation.mutateAsync({ id: receipt.id, payload: { voidReason: voidReason.trim() } });
      toast.success("Receipt marked void. Payment and invoice balances were not changed.");
      setShowVoidForm(false);
      setVoidReason("");
    } catch (error) {
      toast.error(apiErrorMessage(error, "Unable to void receipt."));
    }
  }

  if (!canView) {
    return <Card className="mx-auto max-w-4xl"><CardContent className="p-6 text-sm text-destructive">Receipt view permission is required to access this page.</CardContent></Card>;
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
        <div><h1 className="font-display text-2xl font-semibold">Receipts</h1><p className="text-sm text-muted-foreground">Find and reprint receipts created from collected payments.</p></div>
        <Link to="/fees/payments" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">Payment history</Link>
      </div>

      <Card>
        <CardHeader><CardTitle>Find receipt</CardTitle><CardDescription>Look up by receipt ID or the payment ID that issued it.</CardDescription></CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2">
          <form onSubmit={findByReceiptId} className="flex items-end gap-2"><div className="min-w-0 flex-1"><label htmlFor="receipt-id" className="mb-1 block text-sm font-medium">Receipt ID</label><input id="receipt-id" type="number" min="1" value={receiptIdInput} onChange={(event) => setReceiptIdInput(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div><Button type="submit" variant="outline" className="px-3"><Search className="mr-2 h-4 w-4" />Find</Button></form>
          <form onSubmit={findByPaymentId} className="flex items-end gap-2"><div className="min-w-0 flex-1"><label htmlFor="payment-id" className="mb-1 block text-sm font-medium">Payment ID</label><input id="payment-id" type="number" min="1" value={paymentIdInput} onChange={(event) => setPaymentIdInput(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /></div><Button type="submit" variant="outline" className="px-3"><Search className="mr-2 h-4 w-4" />Find</Button></form>
        </CardContent>
      </Card>

      {(receiptId !== null || paymentId !== null) && <Card>
        <CardHeader><div className="flex flex-wrap items-start justify-between gap-3"><div><CardTitle>Receipt details</CardTitle><CardDescription>Issued receipt record and payment reference.</CardDescription></div>{receipt && <Button type="button" onClick={() => void downloadPdf()} disabled={downloadMutation.isPending}>{downloadMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Download className="mr-2 h-4 w-4" />}Download PDF</Button>}</div></CardHeader>
        <CardContent>
          {isPending && <p className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading receipt…</p>}
          {isError && <p className="rounded-md border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Receipt not found or could not be loaded. Verify the ID and your access.</p>}
          {receipt && <div className="space-y-5">
            {receipt.isVoided && <div className="flex gap-3 rounded-md border border-destructive/30 bg-destructive/5 p-4 text-sm text-destructive"><AlertTriangle className="mt-0.5 h-4 w-4 shrink-0" /><div><p className="font-semibold">VOIDED RECEIPT</p><p>This receipt is void, but the payment and invoice balance are unchanged by this receipt-only action.</p>{receipt.voidReason && <p className="mt-1">Reason: {receipt.voidReason}</p>}</div></div>}
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
              <div><p className="text-xs text-muted-foreground">Receipt number</p><p className={`text-lg font-semibold ${receipt.isVoided ? "text-destructive line-through" : ""}`}>{receipt.receiptNo}</p></div>
              <div><p className="text-xs text-muted-foreground">Payment number</p><p>{receipt.paymentNumber}</p></div>
              <div><p className="text-xs text-muted-foreground">Status</p><span className={`rounded-full px-2 py-1 text-xs font-medium ${receipt.isVoided ? "bg-destructive/10 text-destructive" : "bg-primary/10 text-primary"}`}>{receipt.isVoided ? "Voided" : "Issued"}</span></div>
              <div><p className="text-xs text-muted-foreground">Issued at</p><p>{new Date(receipt.issuedAt).toLocaleString()}</p></div>
              <div><p className="text-xs text-muted-foreground">Issued by</p><p>{receipt.issuedByEmployeeName}</p></div>
              {receipt.voidedAt && <div><p className="text-xs text-muted-foreground">Voided at</p><p>{new Date(receipt.voidedAt).toLocaleString()}</p></div>}
            </div>
            <div className="flex flex-wrap gap-2 border-t pt-4">
              <Link to="/fees/payments" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">Open payments</Link>
              {canVoid && !receipt.isVoided && <Button type="button" variant="outline" className="text-destructive" onClick={() => setShowVoidForm((value) => !value)}><X className="mr-2 h-4 w-4" />Receipt-only void</Button>}
            </div>
            {showVoidForm && canVoid && !receipt.isVoided && <form onSubmit={(event) => void submitVoid(event)} className="space-y-3 rounded-md border border-amber-500/30 bg-amber-500/5 p-4">
              <p className="flex items-start gap-2 text-sm text-amber-900"><AlertTriangle className="mt-0.5 h-4 w-4 shrink-0" />This is an edge-case action for an incorrect receipt record. It does not void the payment, restore invoice balance, or replace payment voiding.</p>
              <div><label htmlFor="receipt-void-reason" className="mb-1 block text-sm font-medium">Reason</label><textarea id="receipt-void-reason" required maxLength={300} rows={2} value={voidReason} onChange={(event) => setVoidReason(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" /><p className="mt-1 text-right text-xs text-muted-foreground">{voidReason.length}/300</p></div>
              <div className="flex gap-2"><Button type="submit" variant="outline" disabled={voidMutation.isPending}>{voidMutation.isPending && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}Confirm receipt-only void</Button><Button type="button" variant="outline" onClick={() => { setShowVoidForm(false); setVoidReason(""); }}>Cancel</Button></div>
            </form>}
          </div>}
        </CardContent>
      </Card>}
    </div>
  );
}