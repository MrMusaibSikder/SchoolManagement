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
  Zap,
} from "lucide-react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { PermissionGuard } from "@/features/auth/components/PermissionGuard";
import { useAuth } from "@/features/auth/hooks/useAuth";
import { Permission } from "@/lib/permissions";
import {
  DashboardQuickActionCard,
  DashboardStatCard,
} from "../components/DashboardCards";
import {
  getDashboardWelcome,
  QUICK_ACTIONS,
} from "../config/dashboard-access";
import {
  useDashboardAccess,
  useDashboardData,
} from "../hooks/useDashboardData";

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
  const {
    access,
    isPending: permissionsPending,
    primaryRole,
    roles,
    hasPermission,
  } = useDashboardAccess();
  const { data, isPending, isError, error, refetch, isFetching } =
    useDashboardData();

  const welcome = getDashboardWelcome(primaryRole);
  const quickActions = QUICK_ACTIONS.filter((action) =>
    hasPermission(action.permission)
  );

  const totalStudents = access.canViewStudentStats
    ? (data?.stats?.totalStudents ?? 0)
    : 0;
  const totalTeachers = access.canViewTeacherStats
    ? (data?.stats?.totalTeachers ?? 0)
    : 0;
  const totalEmployees = access.canViewEmployeeStats
    ? (data?.stats?.totalEmployees ?? 0)
    : 0;

  const totalCollected = (data?.invoices ?? []).reduce((sum, invoice) => {
    const total = typeof invoice.totalAmount === "number" ? invoice.totalAmount : 0;
    const balance = typeof invoice.balanceDue === "number" ? invoice.balanceDue : 0;
    return sum + Math.max(total - balance, 0);
  }, 0);
  const pendingFees = (data?.invoices ?? []).reduce((sum, invoice) => {
    const balance = typeof invoice.balanceDue === "number" ? invoice.balanceDue : 0;
    return sum + balance;
  }, 0);

  const attendanceRate = data?.attendance?.attendancePercentage ?? null;
  const attendanceLabel =
    typeof attendanceRate === "number"
      ? `${attendanceRate.toFixed(1)}%`
      : "—";

  const showSecondaryRow = access.canViewFees || access.canViewExams;
  const showTertiaryRow = access.canViewNotices || access.canViewActivity;
  const loading = permissionsPending || isPending;

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <Card className="overflow-hidden border-primary/20 bg-gradient-to-br from-primary to-primary/90 text-primary-foreground">
        <CardContent className="flex flex-col gap-4 p-6 sm:flex-row sm:items-center sm:justify-between sm:p-8">
          <div>
            <p className="text-sm uppercase tracking-[0.2em] text-primary-foreground/70">
              {welcome.eyebrow}
            </p>
            <h1 className="mt-2 font-display text-2xl font-semibold sm:text-3xl">
              Welcome back, {session?.username ?? "there"} 👋
            </h1>
            <p className="mt-2 max-w-2xl text-sm text-primary-foreground/80">
              {welcome.subtitle}
            </p>
            {roles.length > 0 && (
              <div className="mt-4 flex flex-wrap gap-2">
                {roles.map((role) => (
                  <Badge
                    key={role}
                    variant="secondary"
                    className="bg-primary-foreground/15 text-primary-foreground hover:bg-primary-foreground/20"
                  >
                    {role}
                  </Badge>
                ))}
              </div>
            )}
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

      {quickActions.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Zap aria-hidden="true" className="h-5 w-5 text-muted-foreground" />
              Quick Actions
            </CardTitle>
            <CardDescription>
              Shortcuts based on your role and permissions.
            </CardDescription>
          </CardHeader>
          <CardContent className="grid gap-3 sm:grid-cols-2 xl:grid-cols-3">
            {quickActions.map((action) => (
              <DashboardQuickActionCard key={action.to} {...action} />
            ))}
          </CardContent>
        </Card>
      )}

      {loading ? (
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
            <button
              type="button"
              onClick={() => void refetch()}
              disabled={isFetching}
              className="mt-4 inline-flex items-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent disabled:opacity-60"
            >
              {isFetching ? "Retrying…" : "Try again"}
            </button>
          </CardContent>
        </Card>
      ) : !access.hasAnyWidget ? (
        <Card>
          <CardContent className="p-8 text-center">
            <p className="font-medium">No dashboard widgets are available yet.</p>
            <p className="mt-2 text-sm text-muted-foreground">
              Your account does not have permission to view summary modules. Contact an administrator if you need access.
            </p>
          </CardContent>
        </Card>
      ) : (
        <>
          <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
            <PermissionGuard permission={Permission.StudentView}>
              <DashboardStatCard
                title="Total Students"
                value={formatCount(totalStudents)}
                description="Registered learners"
                icon={GraduationCap}
              />
            </PermissionGuard>

            <PermissionGuard permission={Permission.TeacherView}>
              <DashboardStatCard
                title="Total Teachers"
                value={formatCount(totalTeachers)}
                description="Teaching staff"
                icon={UserCheck}
              />
            </PermissionGuard>

            <PermissionGuard permission={Permission.EmployeeView}>
              <DashboardStatCard
                title="Total Employees"
                value={formatCount(totalEmployees)}
                description="Support and administration"
                icon={Users}
              />
            </PermissionGuard>

            <PermissionGuard permission={Permission.AttendanceReportView}>
              <DashboardStatCard
                title="Attendance Summary"
                value={attendanceLabel}
                description={`${data?.attendance?.totalPresent ?? 0} present / ${data?.attendance?.totalAbsent ?? 0} absent`}
                icon={ClipboardList}
              />
            </PermissionGuard>
          </div>

          {showSecondaryRow && (
            <div
              className={`grid gap-4 ${access.canViewFees && access.canViewExams ? "xl:grid-cols-[1.2fr_0.8fr]" : ""}`}
            >
              <PermissionGuard permission={Permission.InvoiceView}>
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <CircleDollarSign aria-hidden="true" className="h-5 w-5 text-muted-foreground" />
                      Fee Activity
                    </CardTitle>
                    <CardDescription>
                      Collection and pending balances from recent invoices.
                    </CardDescription>
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
                        <div className="px-4 py-6 text-sm text-muted-foreground">
                          No invoice data available.
                        </div>
                      ) : (
                        (data?.invoices ?? []).slice(0, 5).map((invoice) => (
                          <div
                            key={invoice.id ?? invoice.invoiceNumber}
                            className="grid grid-cols-[1fr_auto_auto] items-center border-b px-4 py-3 text-sm last:border-b-0"
                          >
                            <div>
                              <p className="font-medium">
                                {invoice.invoiceNumber ?? `Invoice #${invoice.id ?? "—"}`}
                              </p>
                              <p className="text-xs text-muted-foreground">
                                {invoice.studentName ?? "Student"}
                              </p>
                            </div>
                            <span className="text-right">
                              {formatCurrency(
                                Math.max(
                                  (invoice.totalAmount ?? 0) - (invoice.balanceDue ?? 0),
                                  0
                                )
                              )}
                            </span>
                            <span className="text-right">
                              {formatCurrency(invoice.balanceDue ?? 0)}
                            </span>
                          </div>
                        ))
                      )}
                    </div>
                  </CardContent>
                </Card>
              </PermissionGuard>

              <PermissionGuard permission={Permission.ExamView}>
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
                      <div className="rounded-lg border border-dashed p-4 text-sm text-muted-foreground">
                        No upcoming exams available.
                      </div>
                    ) : (
                      (data?.upcomingExams ?? []).slice(0, 5).map((exam) => (
                        <div key={exam.examId ?? exam.examName} className="rounded-lg border p-3">
                          <p className="font-medium">{exam.examName ?? "Exam"}</p>
                          <p className="mt-1 text-sm text-muted-foreground">
                            {exam.examTypeName ?? "Exam"} • {exam.daysRemaining ?? 0} days left
                          </p>
                          <p className="mt-2 text-xs text-muted-foreground">
                            Next date:{" "}
                            {exam.nextExamDate
                              ? new Date(exam.nextExamDate).toLocaleDateString("en-BD")
                              : "—"}
                          </p>
                        </div>
                      ))
                    )}
                  </CardContent>
                </Card>
              </PermissionGuard>
            </div>
          )}

          {showTertiaryRow && (
            <div
              className={`grid gap-4 ${access.canViewNotices && access.canViewActivity ? "lg:grid-cols-[1fr_0.95fr]" : ""}`}
            >
              <PermissionGuard permission={Permission.NoticeView}>
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
                      <div className="rounded-lg border border-dashed p-4 text-sm text-muted-foreground">
                        No notices available.
                      </div>
                    ) : (
                      (data?.notices ?? []).slice(0, 5).map((notice) => (
                        <div key={notice.id} className="rounded-lg border p-3">
                          <div className="flex items-start justify-between gap-3">
                            <div>
                              <p className="font-medium">{notice.title ?? "Notice"}</p>
                              <p className="mt-1 text-sm text-muted-foreground">
                                {notice.description
                                  ? notice.description.slice(0, 120)
                                  : "No description available."}
                                {notice.description && notice.description.length > 120 ? "…" : ""}
                              </p>
                            </div>
                            <span className="rounded-full bg-muted px-2.5 py-1 text-[11px] uppercase tracking-wide text-muted-foreground">
                              {notice.priority ?? "General"}
                            </span>
                          </div>
                          <p className="mt-2 text-xs text-muted-foreground">
                            Published{" "}
                            {notice.publishDate
                              ? new Date(notice.publishDate).toLocaleDateString("en-BD")
                              : "—"}
                          </p>
                        </div>
                      ))
                    )}
                  </CardContent>
                </Card>
              </PermissionGuard>

              <PermissionGuard anyOf={[Permission.StudentView, Permission.InvoiceView]}>
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <ReceiptText aria-hidden="true" className="h-5 w-5 text-muted-foreground" />
                      Recent Activity
                    </CardTitle>
                    <CardDescription>A quick snapshot based on your access.</CardDescription>
                  </CardHeader>
                  <CardContent className="space-y-3">
                    <PermissionGuard permission={Permission.StudentView}>
                      <div className="rounded-lg border p-3">
                        <p className="text-sm font-medium">Student overview</p>
                        <p className="mt-1 text-sm text-muted-foreground">
                          {totalStudents > 0
                            ? `${totalStudents} students are currently registered.`
                            : "No students available yet."}
                        </p>
                      </div>
                    </PermissionGuard>
                    <PermissionGuard permission={Permission.InvoiceView}>
                      <div className="rounded-lg border p-3">
                        <p className="text-sm font-medium">Payment overview</p>
                        <p className="mt-1 text-sm text-muted-foreground">
                          {totalCollected > 0
                            ? `${formatCurrency(totalCollected)} collected across recent invoices.`
                            : "No payment activity available yet."}
                        </p>
                      </div>
                    </PermissionGuard>
                  </CardContent>
                </Card>
              </PermissionGuard>
            </div>
          )}
        </>
      )}
    </div>
  );
}
