import { Link } from "react-router-dom";
import {
  CalendarDays,
  CircleDollarSign,
  ClipboardList,
  GraduationCap,
  KeyRound,
  Loader2,
  MessageSquareWarning,
  ReceiptText,
  UserCheck,
  Users,
} from "lucide-react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { useAuth } from "@/features/auth/hooks/useAuth";
import { useDashboardData } from "../hooks/useDashboardData";

function formatCurrency(value: number | null | undefined) {
  if (typeof value !== "number" || Number.isNaN(value)) return "—";
  return new Intl.NumberFormat("en-BD", {
    style: "currency",
    currency: "BDT",
    maximumFractionDigits: 0,
  }).format(value);
}

function formatCount(value: number | null | undefined) {
  if (typeof value !== "number" || Number.isNaN(value)) return "—";
  return value.toLocaleString("en-BD");
}

export function DashboardPage() {
  const { session } = useAuth();
  const { data, isPending, isError, error } = useDashboardData();

  const totalStudents = data?.students?.length ?? 0;
  const totalTeachers = data?.teachers?.length ?? 0;
  const totalEmployees = data?.employees?.length ?? 0;

  const totalCollected = (data?.invoices ?? []).reduce((sum, invoice) => {
    const paid = typeof invoice.amountPaid === "number" ? invoice.amountPaid : 0;
    return sum + paid;
  }, 0);
  const pendingFees = (data?.invoices ?? []).reduce((sum, invoice) => {
    const balance = typeof invoice.balanceDue === "number" ? invoice.balanceDue : 0;
    return sum + balance;
  }, 0);

  const attendanceRate = data?.attendance?.attendanceRate ?? null;
  const attendanceLabel =
    typeof attendanceRate === "number"
      ? `${attendanceRate.toFixed(1)}%`
      : "—";

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <Card className="overflow-hidden border-primary/20 bg-gradient-to-br from-primary to-primary/90 text-primary-foreground">
        <CardContent className="flex flex-col gap-4 p-6 sm:flex-row sm:items-center sm:justify-between sm:p-8">
          <div>
            <p className="text-sm uppercase tracking-[0.2em] text-primary-foreground/70">
              School overview
            </p>
            <h1 className="mt-2 font-display text-2xl font-semibold sm:text-3xl">
              Welcome back, {session?.username ?? "there"} 👋
            </h1>
            <p className="mt-2 max-w-2xl text-sm text-primary-foreground/80">
              Review student growth, staffing, fee activity, exams, and notices from one place.
            </p>
          </div>
          <Link
            to="/change-password"
            className="inline-flex shrink-0 items-center gap-2 rounded-md bg-background px-4 py-2 text-sm font-medium text-primary shadow-sm transition hover:bg-background/90"
          >
            <KeyRound aria-hidden="true" className="h-4 w-4" />
            Change Password
          </Link>
        </CardContent>
      </Card>

      {isPending ? (
        <div className="flex min-h-48 items-center justify-center rounded-xl border border-dashed bg-card/70 p-8 text-muted-foreground">
          <div className="flex items-center gap-3">
            <Loader2 aria-hidden="true" className="h-5 w-5 animate-spin" />
            Loading dashboard data…
          </div>
        </div>
      ) : isError ? (
        <Card className="border-destructive/20 bg-destructive/5">
          <CardContent className="p-6">
            <p className="font-medium text-destructive">
              We couldn’t load the dashboard right now.
            </p>
            <p className="mt-2 text-sm text-muted-foreground">
              {error instanceof Error ? error.message : "Please try again in a moment."}
            </p>
          </CardContent>
        </Card>
      ) : (
        <>
          <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
            <Card>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">Total Students</CardTitle>
                <span className="flex h-9 w-9 items-center justify-center rounded-lg bg-muted text-muted-foreground">
                  <GraduationCap aria-hidden="true" className="h-5 w-5" />
                </span>
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-semibold tabular-nums">{formatCount(totalStudents)}</div>
                <p className="mt-1 text-xs text-muted-foreground">Registered learners</p>
              </CardContent>
            </Card>

            <Card>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">Total Teachers</CardTitle>
                <span className="flex h-9 w-9 items-center justify-center rounded-lg bg-muted text-muted-foreground">
                  <UserCheck aria-hidden="true" className="h-5 w-5" />
                </span>
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-semibold tabular-nums">{formatCount(totalTeachers)}</div>
                <p className="mt-1 text-xs text-muted-foreground">Teaching staff</p>
              </CardContent>
            </Card>

            <Card>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">Total Employees</CardTitle>
                <span className="flex h-9 w-9 items-center justify-center rounded-lg bg-muted text-muted-foreground">
                  <Users aria-hidden="true" className="h-5 w-5" />
                </span>
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-semibold tabular-nums">{formatCount(totalEmployees)}</div>
                <p className="mt-1 text-xs text-muted-foreground">Support and administration</p>
              </CardContent>
            </Card>

            <Card>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">Attendance Summary</CardTitle>
                <span className="flex h-9 w-9 items-center justify-center rounded-lg bg-muted text-muted-foreground">
                  <ClipboardList aria-hidden="true" className="h-5 w-5" />
                </span>
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-semibold tabular-nums">{attendanceLabel}</div>
                <p className="mt-1 text-xs text-muted-foreground">
                  {data?.attendance?.presentCount ?? 0} present / {data?.attendance?.absentCount ?? 0} absent
                </p>
              </CardContent>
            </Card>
          </div>

          <div className="grid gap-4 xl:grid-cols-[1.2fr_0.8fr]">
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <CircleDollarSign aria-hidden="true" className="h-5 w-5 text-muted-foreground" />
                  Fee Activity
                </CardTitle>
                <CardDescription>Collection and pending balances from recent invoices.</CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="grid gap-3 sm:grid-cols-2">
                  <div className="rounded-lg border bg-muted/40 p-4">
                    <p className="text-sm text-muted-foreground">Fee Collection</p>
                    <p className="mt-2 text-2xl font-semibold">{formatCurrency(totalCollected)}</p>
                  </div>
                  <div className="rounded-lg border bg-muted/40 p-4">
                    <p className="text-sm text-muted-foreground">Pending Fees</p>
                    <p className="mt-2 text-2xl font-semibold">{formatCurrency(pendingFees)}</p>
                  </div>
                </div>
                <div className="max-h-72 overflow-auto rounded-lg border">
                  <div className="grid grid-cols-[1fr_auto_auto] border-b bg-muted/40 px-4 py-3 text-sm font-medium text-muted-foreground">
                    <span>Invoice</span>
                    <span className="text-right">Paid</span>
                    <span className="text-right">Balance</span>
                  </div>
                  {(data?.invoices ?? []).length === 0 ? (
                    <div className="px-4 py-6 text-sm text-muted-foreground">No invoice data available.</div>
                  ) : (
                    (data?.invoices ?? []).slice(0, 5).map((invoice) => (
                      <div key={invoice.id ?? invoice.invoiceNumber} className="grid grid-cols-[1fr_auto_auto] items-center border-b px-4 py-3 text-sm last:border-b-0">
                        <div>
                          <p className="font-medium">{invoice.invoiceNumber ?? `Invoice #${invoice.id ?? "—"}`}</p>
                          <p className="text-xs text-muted-foreground">{invoice.studentName ?? "Student"}</p>
                        </div>
                        <span className="text-right">{formatCurrency(invoice.amountPaid ?? 0)}</span>
                        <span className="text-right">{formatCurrency(invoice.balanceDue ?? 0)}</span>
                      </div>
                    ))
                  )}
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <CalendarDays aria-hidden="true" className="h-5 w-5 text-muted-foreground" />
                  Upcoming Exams
                </CardTitle>
                <CardDescription>Next scheduled examinations.</CardDescription>
              </CardHeader>
              <CardContent className="space-y-3">
                {(data?.upcomingExams ?? []).length === 0 ? (
                  <div className="rounded-lg border border-dashed p-4 text-sm text-muted-foreground">No upcoming exams available.</div>
                ) : (
                  (data?.upcomingExams ?? []).slice(0, 5).map((exam) => (
                    <div key={exam.examId ?? exam.examName} className="rounded-lg border p-3">
                      <p className="font-medium">{exam.examName ?? "Exam"}</p>
                      <p className="mt-1 text-sm text-muted-foreground">
                        {exam.examTypeName ?? "Exam"} • {exam.daysRemaining ?? 0} days left
                      </p>
                      <p className="mt-2 text-xs text-muted-foreground">
                        Next date: {exam.nextExamDate ? new Date(exam.nextExamDate).toLocaleDateString("en-BD") : "—"}
                      </p>
                    </div>
                  ))
                )}
              </CardContent>
            </Card>
          </div>

          <div className="grid gap-4 lg:grid-cols-[1fr_0.95fr]">
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <MessageSquareWarning aria-hidden="true" className="h-5 w-5 text-muted-foreground" />
                  Recent Notices
                </CardTitle>
                <CardDescription>Latest school communications.</CardDescription>
              </CardHeader>
              <CardContent className="space-y-3">
                {(data?.notices ?? []).length === 0 ? (
                  <div className="rounded-lg border border-dashed p-4 text-sm text-muted-foreground">No notices available.</div>
                ) : (
                  (data?.notices ?? []).slice(0, 5).map((notice) => (
                    <div key={notice.id} className="rounded-lg border p-3">
                      <div className="flex items-start justify-between gap-3">
                        <div>
                          <p className="font-medium">{notice.title ?? "Notice"}</p>
                          <p className="mt-1 text-sm text-muted-foreground">
                            {notice.description ? notice.description.slice(0, 120) : "No description available."}
                            {notice.description && notice.description.length > 120 ? "…" : ""}
                          </p>
                        </div>
                        <span className="rounded-full bg-muted px-2.5 py-1 text-[11px] uppercase tracking-wide text-muted-foreground">
                          {notice.priority ?? "General"}
                        </span>
                      </div>
                      <p className="mt-2 text-xs text-muted-foreground">
                        Published {notice.publishDate ? new Date(notice.publishDate).toLocaleDateString("en-BD") : "—"}
                      </p>
                    </div>
                  ))
                )}
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <ReceiptText aria-hidden="true" className="h-5 w-5 text-muted-foreground" />
                  Recent Admissions & Payments
                </CardTitle>
                <CardDescription>A quick snapshot of recent activity.</CardDescription>
              </CardHeader>
              <CardContent className="space-y-3">
                <div className="rounded-lg border p-3">
                  <p className="text-sm font-medium">Recent admissions</p>
                  <p className="mt-1 text-sm text-muted-foreground">
                    {totalStudents > 0 ? `${totalStudents} students are currently registered.` : "No students available yet."}
                  </p>
                </div>
                <div className="rounded-lg border p-3">
                  <p className="text-sm font-medium">Recent payments</p>
                  <p className="mt-1 text-sm text-muted-foreground">
                    {totalCollected > 0 ? `${formatCurrency(totalCollected)} collected across recent invoices.` : "No payment activity available yet."}
                  </p>
                </div>
              </CardContent>
            </Card>
          </div>
        </>
      )}
    </div>
  );
}
