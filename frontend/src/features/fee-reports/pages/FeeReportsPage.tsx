import { useState } from "react";
import { Link } from "react-router-dom";
import { Loader2 } from "lucide-react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useSchoolClasses } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { useCollectionSummaryReport, useDefaulterReport } from "../hooks/useFeeReportData";

function money(value: number) {
  return new Intl.NumberFormat("en-BD", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value);
}

function daysAgo(days: number) {
  const date = new Date();
  date.setDate(date.getDate() - days);
  return date.toISOString().slice(0, 10);
}

function todayIso() {
  return new Date().toISOString().slice(0, 10);
}

export function FeeReportsPage() {
  const { hasPermission } = usePermissions();
  const canViewCollection = hasPermission(Permission.PaymentView);
  const canViewDefaulters = hasPermission(Permission.InvoiceView);
  const { data: classes = [] } = useSchoolClasses();

  const [dateFrom, setDateFrom] = useState(daysAgo(30));
  const [dateTo, setDateTo] = useState(todayIso());
  const [asOfDate, setAsOfDate] = useState(todayIso());
  const [schoolClassId, setSchoolClassId] = useState("");

  const collectionReport = useCollectionSummaryReport(dateFrom, dateTo, canViewCollection);
  const defaulterReport = useDefaulterReport(asOfDate, schoolClassId ? Number(schoolClassId) : null, canViewDefaulters);

  if (!canViewCollection && !canViewDefaulters) {
    return <Card className="mx-auto max-w-4xl"><CardContent className="p-6 text-sm text-destructive">You do not have permission to view fee reports.</CardContent></Card>;
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Fee reports</h1>
          <p className="text-sm text-muted-foreground">Collection performance and overdue defaulter summaries.</p>
        </div>
        <Link to="/fees/invoices" className="inline-flex items-center justify-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent">Invoices</Link>
      </div>

      {canViewCollection && <Card>
        <CardHeader>
          <CardTitle>Collection summary</CardTitle>
          <CardDescription>Payment totals and daily trend for a chosen date range.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 md:grid-cols-2">
            <input type="date" value={dateFrom} onChange={(event) => setDateFrom(event.target.value)} className="rounded-md border bg-background px-3 py-2" />
            <input type="date" value={dateTo} onChange={(event) => setDateTo(event.target.value)} className="rounded-md border bg-background px-3 py-2" />
          </div>

          {collectionReport.isPending ? (
            <div className="flex items-center text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading collection summary…</div>
          ) : collectionReport.isError || !collectionReport.data ? (
            <p className="text-sm text-destructive">Unable to load collection summary.</p>
          ) : (
            <div className="space-y-5">
              <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
                <Stat label="Collected" value={`BDT ${money(collectionReport.data.totalCollected)}`} />
                <Stat label="Transactions" value={String(collectionReport.data.totalTransactions)} />
                <Stat label="Avg. per payment" value={`BDT ${money(collectionReport.data.averageTransactionAmount)}`} />
                <Stat label="Date range" value={`${collectionReport.data.dateFrom} → ${collectionReport.data.dateTo}`} />
              </div>

              <div className="grid gap-6 lg:grid-cols-2">
                <div className="overflow-x-auto rounded-lg border">
                  <table className="w-full text-left text-sm">
                    <thead className="border-b bg-muted/40"><tr><th className="px-3 py-2 font-medium">Date</th><th className="px-3 py-2 font-medium">Collected</th><th className="px-3 py-2 font-medium">Txns</th></tr></thead>
                    <tbody>{collectionReport.data.dailyBreakdown.length ? collectionReport.data.dailyBreakdown.map((item) => (
                      <tr key={item.date} className="border-b last:border-0"><td className="px-3 py-2">{item.date}</td><td className="px-3 py-2">BDT {money(item.collected)}</td><td className="px-3 py-2">{item.transactions}</td></tr>
                    )) : <tr><td colSpan={3} className="px-3 py-4 text-muted-foreground">No payment activity in this range.</td></tr>}</tbody>
                  </table>
                </div>

                <div className="overflow-x-auto rounded-lg border">
                  <table className="w-full text-left text-sm">
                    <thead className="border-b bg-muted/40"><tr><th className="px-3 py-2 font-medium">Method</th><th className="px-3 py-2 font-medium">Amount</th><th className="px-3 py-2 font-medium">Count</th></tr></thead>
                    <tbody>{collectionReport.data.methodBreakdown.length ? collectionReport.data.methodBreakdown.map((item) => (
                      <tr key={item.method} className="border-b last:border-0"><td className="px-3 py-2">{item.method}</td><td className="px-3 py-2">BDT {money(item.amount)}</td><td className="px-3 py-2">{item.count}</td></tr>
                    )) : <tr><td colSpan={3} className="px-3 py-4 text-muted-foreground">No method breakdown available.</td></tr>}</tbody>
                  </table>
                </div>
              </div>
            </div>
          )}
        </CardContent>
      </Card>}

      {canViewDefaulters && <Card>
        <CardHeader>
          <CardTitle>Defaulter report</CardTitle>
          <CardDescription>Overdue invoices grouped by student, sorted by balance.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 md:grid-cols-2">
            <input type="date" value={asOfDate} onChange={(event) => setAsOfDate(event.target.value)} className="rounded-md border bg-background px-3 py-2" />
            <select value={schoolClassId} onChange={(event) => setSchoolClassId(event.target.value)} className="rounded-md border bg-background px-3 py-2">
              <option value="">All classes</option>
              {classes.map((classItem) => <option key={classItem.id} value={classItem.id}>{classItem.name}</option>)}
            </select>
          </div>

          {defaulterReport.isPending ? (
            <div className="flex items-center text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading defaulters…</div>
          ) : defaulterReport.isError || !defaulterReport.data ? (
            <p className="text-sm text-destructive">Unable to load defaulter report.</p>
          ) : (
            <div className="space-y-4">
              <div className="grid gap-3 sm:grid-cols-3">
                <Stat label="Defaulters" value={String(defaulterReport.data.totalDefaulters)} />
                <Stat label="Overdue amount" value={`BDT ${money(defaulterReport.data.totalOverdueAmount)}`} />
                <Stat label="As of" value={defaulterReport.data.asOfDate} />
              </div>

              <div className="overflow-x-auto rounded-lg border">
                <table className="w-full min-w-[760px] text-left text-sm">
                  <thead className="border-b bg-muted/40">
                    <tr>
                      <th className="px-3 py-2 font-medium">Student</th>
                      <th className="px-3 py-2 font-medium">Class</th>
                      <th className="px-3 py-2 font-medium">Invoices</th>
                      <th className="px-3 py-2 font-medium">Overdue amount</th>
                      <th className="px-3 py-2 font-medium">Oldest due</th>
                      <th className="px-3 py-2 font-medium">Days overdue</th>
                    </tr>
                  </thead>
                  <tbody>{defaulterReport.data.defaulters.length ? defaulterReport.data.defaulters.map((item) => (
                    <tr key={item.studentId} className="border-b last:border-0">
                      <td className="px-3 py-2">{item.studentName}</td>
                      <td className="px-3 py-2">{item.schoolClassName ?? "—"}</td>
                      <td className="px-3 py-2">{item.overdueInvoiceCount}</td>
                      <td className="px-3 py-2">BDT {money(item.totalOverdueAmount)}</td>
                      <td className="px-3 py-2">{item.oldestDueDate}</td>
                      <td className="px-3 py-2">{item.daysOverdue}</td>
                    </tr>
                  )) : <tr><td colSpan={6} className="px-3 py-4 text-muted-foreground">No defaulters found for the selected filters.</td></tr>}</tbody>
                </table>
              </div>
            </div>
          )}
        </CardContent>
      </Card>}
    </div>
  );
}

function Stat({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-md border bg-muted/20 p-3">
      <p className="text-xs uppercase tracking-wide text-muted-foreground">{label}</p>
      <p className="mt-2 text-lg font-semibold">{value}</p>
    </div>
  );
}
