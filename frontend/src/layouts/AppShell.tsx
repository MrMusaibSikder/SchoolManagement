import { useMemo, useState } from "react";
import { NavLink, Outlet, useNavigate } from "react-router-dom";
import {
  ArrowRightLeft,
  BellRing,
  BookOpen,
  BookMarked,
  Briefcase,
  ClipboardPenLine,
  ClipboardList,
  GraduationCap,
  LayoutDashboard,
  KeyRound,
  LogOut,
  Menu,
  MessageSquareText,
  PiggyBank,
  ReceiptText,
  School,
  Settings,
  Users,
  UserRound,
  UsersRound,
  X,
} from "lucide-react";
import { toast } from "sonner";
import { cn } from "@/lib/utils";
import { AppRole, Permission } from "@/lib/permissions";
import { Avatar } from "@/components/ui/avatar";
import { SchoolLogo } from "@/components/common/SchoolLogo";
import { Badge } from "@/components/ui/badge";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { useAuth } from "@/features/auth/hooks/useAuth";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { useSchoolSettings } from "@/features/settings/hooks/useSettingsData";
import { DEFAULT_SCHOOL_NAME } from "@/config/branding";

const NAV_ITEMS = [
  { to: "/dashboard", label: "Dashboard", icon: LayoutDashboard },
  {
    to: "/academic",
    label: "Academic",
    icon: School,
    permission: Permission.AcademicYearView,
  },
  {
    to: "/academic/classes",
    label: "Classes & Sections",
    icon: GraduationCap,
    anyOf: [
      Permission.SchoolClassView,
      Permission.SchoolClassCreate,
      Permission.SchoolClassEdit,
      Permission.SchoolClassDelete,
      Permission.SectionView,
      Permission.SectionCreate,
      Permission.SectionEdit,
      Permission.SectionDelete,
    ],
  },
  {
    to: "/academic/class-subjects",
    label: "Class Subjects",
    icon: BookMarked,
    anyOf: [Permission.ClassSubjectView, Permission.ClassSubjectAssign, Permission.ClassSubjectRemove],
  },
  {
    to: "/academic/subjects",
    label: "Subjects",
    icon: BookOpen,
    anyOf: [Permission.SubjectView, Permission.SubjectCreate, Permission.SubjectEdit, Permission.SubjectDelete],
  },
  {
    to: "/academic/teachers",
    label: "Teachers",
    icon: UsersRound,
    permission: Permission.TeacherView,
  },
  {
    to: "/academic/teacher-assignments",
    label: "Teacher Assignments",
    icon: UserRound,
    anyOf: [Permission.SubjectTeacherView, Permission.SubjectTeacherAssign, Permission.SubjectTeacherRemove],
  },
  {
    to: "/students",
    label: "Students",
    icon: Users,
    permission: Permission.StudentView,
  },
  {
    to: "/notices",
    label: "Notices",
    icon: BellRing,
    anyOf: [Permission.NoticeView, Permission.NoticeCreate],
  },
  {
    to: "/sms",
    label: "SMS management",
    icon: MessageSquareText,
    anyOf: [
      Permission.SmsTemplateView,
      Permission.SmsTemplateCreate,
      Permission.SmsTemplateEdit,
      Permission.SmsTemplateDelete,
      Permission.SmsLogView,
    ],
  },
  {
    to: "/employees",
    label: "Employees",
    icon: Briefcase,
    permission: Permission.EmployeeView,
  },
  {
    to: "/guardians",
    label: "Guardians",
    icon: UserRound,
    permission: Permission.GuardianView,
  },
  {
    to: "/attendance",
    label: "Attendance",
    icon: ClipboardList,
    anyOf: [
      Permission.StudentAttendanceView,
      Permission.StudentAttendanceCreate,
      Permission.EmployeeAttendanceView,
      Permission.EmployeeAttendanceCreate,
      Permission.AttendanceReportView,
    ],
  },
  {
    to: "/roles",
    label: "Roles & Access",
    icon: KeyRound,
    adminOnly: true,
  },
  {
    to: "/fees/categories",
    label: "Fees",
    icon: PiggyBank,
    anyOf: [Permission.FeeCategoryView, Permission.FeeTypeView, Permission.FeeStructureView, Permission.ConcessionView, Permission.ConcessionCreate, Permission.ConcessionApprove, Permission.InvoiceView, Permission.InvoiceCreate, Permission.PaymentView, Permission.PaymentCollect, Permission.PaymentVoid, Permission.ReceiptView, Permission.ReceiptVoid, Permission.LateFineRuleView, Permission.LateFineRuleManage],
  },
  {
    to: "/fees/types",
    label: "Fee Types",
    icon: Badge,
    permission: Permission.FeeTypeView,
  },
  {
    to: "/fees/structures",
    label: "Fee Structures",
    icon: ClipboardList,
    permission: Permission.FeeStructureView,
  },
  {
    to: "/fees/concessions",
    label: "Student Concessions",
    icon: PiggyBank,
    anyOf: [Permission.ConcessionView, Permission.ConcessionCreate, Permission.ConcessionApprove],
  },
  {
    to: "/fees/invoices",
    label: "Invoices",
    icon: ReceiptText,
    anyOf: [Permission.InvoiceView, Permission.InvoiceCreate, Permission.InvoiceCancel],
  },
  {
    to: "/fees/payments",
    label: "Payments",
    icon: ReceiptText,
    anyOf: [Permission.PaymentView, Permission.PaymentCollect, Permission.PaymentVoid],
  },
  {
    to: "/fees/receipts",
    label: "Receipts",
    icon: ReceiptText,
    anyOf: [Permission.ReceiptView, Permission.ReceiptVoid],
  },
  {
    to: "/fees/late-fine-rules",
    label: "Late Fine Rules",
    icon: ClipboardList,
    anyOf: [Permission.LateFineRuleView, Permission.LateFineRuleManage],
  },
  {
    to: "/fees/reports",
    label: "Fee Reports",
    icon: ArrowRightLeft,
    anyOf: [Permission.PaymentView, Permission.InvoiceView, Permission.FeeReportView],
  },
  {
    to: "/exams",
    label: "Exams",
    icon: BookOpen,
    anyOf: [
      Permission.ExamView,
      Permission.ExamTypeView,
      Permission.ExamScheduleView,
      Permission.GradeSetupView,
      Permission.WeightSetupView,
      Permission.MarksEntryView,
      Permission.ResultView,
      Permission.FinalResultView,
      Permission.ResultAuditView,
    ],
  },
  {
    to: "/results/marks",
    label: "Marks entry",
    icon: ClipboardPenLine,
    permission: Permission.MarksEntryView,
  },
] as const;

function getSidebarLinkClass(isActive: boolean) {
  return cn(
    "group flex items-center gap-3 rounded-md border-l-2 px-3 py-2 text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-sidebar-ring",
    isActive
      ? "border-sidebar-primary bg-sidebar-accent text-sidebar-accent-foreground shadow-sm"
      : "border-transparent text-sidebar-foreground/75 hover:bg-sidebar-accent/70 hover:text-sidebar-foreground"
  );
}

export function AppShell() {
  const { session, logout } = useAuth();
  const { data: school } = useSchoolSettings();
  const { hasPermission, hasAnyPermission, hasRole, isPending: permissionsPending } = usePermissions();
  const navigate = useNavigate();
  const [mobileOpen, setMobileOpen] = useState(false);
  const [loggingOut, setLoggingOut] = useState(false);
  const schoolName = school?.name?.trim() || DEFAULT_SCHOOL_NAME;

  const visibleNavItems = useMemo(
    () =>
      NAV_ITEMS.filter((item) => {
        if ("adminOnly" in item && item.adminOnly) return hasRole(AppRole.Admin);
        if ("anyOf" in item) return hasAnyPermission([...item.anyOf]);
        if ("permission" in item) return hasPermission(item.permission);
        return true;
      }),
    [hasAnyPermission, hasPermission, hasRole]
  );

  async function handleLogout() {
    if (loggingOut) return;
    setLoggingOut(true);
    try {
      await logout();
      toast.success("You have been signed out.");
    } catch {
      toast.error("Could not sign out. Please try again.");
    } finally {
      setLoggingOut(false);
    }
  }

  const sidebar = (
    <div className="flex h-full flex-col bg-sidebar text-sidebar-foreground">
      <div className="flex min-h-20 items-center gap-3 border-b border-sidebar-border px-5 py-3">
        <SchoolLogo logo={school?.logo} alt={`${schoolName} logo`} />
        <div className="min-w-0">
          <p className="truncate font-display text-sm font-semibold text-sidebar-foreground">
            {schoolName}
          </p>
          <p className="mt-0.5 truncate text-[10px] font-medium uppercase tracking-[0.18em] text-sidebar-foreground/55">
            School management
          </p>
        </div>
      </div>

      <nav className="flex-1 space-y-1 overflow-y-auto p-4">
        <p className="px-2 pb-2 text-[10px] font-semibold uppercase tracking-[0.2em] text-sidebar-foreground/55">
          Main
        </p>
        {visibleNavItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) => getSidebarLinkClass(isActive)}
          >
            <item.icon aria-hidden="true" className="h-4 w-4 shrink-0 opacity-85 group-hover:opacity-100" />
            {item.label}
          </NavLink>
        ))}

        <p className="px-2 pb-2 pt-6 text-[10px] font-semibold uppercase tracking-[0.2em] text-sidebar-foreground/55">
          Account
        </p>
        <NavLink
          to="/profile"
          className={({ isActive }) => getSidebarLinkClass(isActive)}
        >
          <UserRound aria-hidden="true" className="h-4 w-4 shrink-0 opacity-85" />
          My profile
        </NavLink>
        {hasPermission(Permission.SchoolView) && (
          <NavLink
            to="/settings"
            className={({ isActive }) => getSidebarLinkClass(isActive)}
          >
            <Settings aria-hidden="true" className="h-4 w-4 shrink-0 opacity-85" />
            School settings
          </NavLink>
        )}
        <NavLink
          to="/change-password"
          className={({ isActive }) => getSidebarLinkClass(isActive)}
        >
          <KeyRound aria-hidden="true" className="h-4 w-4 shrink-0 opacity-85" />
          Change Password
        </NavLink>
      </nav>

      <div className="border-t border-sidebar-border p-4">
        <div className="flex items-center gap-3">
          <Avatar name={session?.username} />
          <div className="min-w-0 flex-1">
            <p className="truncate text-sm font-medium text-sidebar-foreground">
              {session?.username}
            </p>
            <p className="truncate text-xs text-sidebar-foreground/60">
              {permissionsPending ? "Loading access…" : (session?.roles?.[0] ?? "User")}
            </p>
          </div>
        </div>
        <button
          type="button"
          onClick={handleLogout}
          disabled={loggingOut}
          className="mt-3 flex w-full items-center justify-center gap-2 rounded-md border border-sidebar-foreground/20 px-3 py-2 text-sm font-medium text-sidebar-foreground/85 transition hover:border-destructive/50 hover:bg-destructive/10 hover:text-red-200 disabled:opacity-60"
        >
          {loggingOut ? (
            <span className="h-4 w-4 animate-spin rounded-full border-2 border-destructive/30 border-t-destructive" />
          ) : (
            <LogOut aria-hidden="true" className="h-4 w-4" />
          )}
          {loggingOut ? "Signing out…" : "Logout"}
        </button>
      </div>
    </div>
  );

  return (
    <div className="min-h-screen bg-background">
      {/* Desktop sidebar */}
      <aside className="fixed inset-y-0 left-0 z-30 hidden w-64 border-r border-sidebar-border shadow-xl lg:block">
        {sidebar}
      </aside>

      {/* Mobile drawer */}
      {mobileOpen && (
        <div className="fixed inset-0 z-40 lg:hidden">
          <div
            className="absolute inset-0 bg-black/50"
            onClick={() => setMobileOpen(false)}
            aria-hidden="true"
          />
          <div className="absolute inset-y-0 left-0 w-72 bg-sidebar shadow-2xl">
            <button
              type="button"
              onClick={() => setMobileOpen(false)}
              className="absolute right-3 top-4 z-10 rounded-md p-1 text-sidebar-foreground/70 hover:bg-sidebar-accent hover:text-sidebar-foreground"
              aria-label="Close menu"
            >
              <X aria-hidden="true" className="h-5 w-5" />
            </button>
            {sidebar}
          </div>
        </div>
      )}

      {/* Main column */}
      <div className="lg:pl-64">
        <header className="sticky top-0 z-20 flex h-16 items-center justify-between border-b-2 border-b-secondary bg-card/95 px-4 shadow-sm backdrop-blur sm:px-6">
          <div className="flex items-center gap-3">
            <button
              type="button"
              onClick={() => setMobileOpen(true)}
              className="rounded-md p-2 text-muted-foreground hover:bg-accent hover:text-foreground lg:hidden"
              aria-label="Open menu"
            >
              <Menu aria-hidden="true" className="h-5 w-5" />
            </button>
            <SchoolLogo
              logo={school?.logo}
              alt={`${schoolName} logo`}
              className="h-9 w-9 rounded-lg ring-1 ring-primary/15"
            />
            <div className="min-w-0">
              <p className="max-w-[min(60vw,28rem)] truncate text-sm font-semibold text-foreground">
                {schoolName}
              </p>
              <p className="hidden text-[10px] font-medium uppercase tracking-[0.16em] text-muted-foreground sm:block">
                School Management System
              </p>
            </div>
          </div>

          <DropdownMenu>
            <DropdownMenuTrigger
              asChild
              className="cursor-pointer rounded-full outline-none focus-visible:ring-2 focus-visible:ring-ring"
            >
              <button type="button" aria-label="Account menu">
                <Avatar name={session?.username} />
              </button>
            </DropdownMenuTrigger>
            <DropdownMenuContent className="w-60">
              <DropdownMenuLabel>
                <div className="flex flex-col">
                  <span className="text-sm font-semibold">
                    {session?.username}
                  </span>
                  <span className="text-xs font-normal text-muted-foreground">
                    {session?.email}
                  </span>
                </div>
              </DropdownMenuLabel>
              {session?.roles && session.roles.length > 0 && (
                <div className="px-2 pb-1">
                  {session.roles.map((r) => (
                    <Badge key={r} variant="secondary" className="mr-1">
                      {r}
                    </Badge>
                  ))}
                </div>
              )}
              <DropdownMenuSeparator />
              <DropdownMenuItem onSelect={() => navigate("/dashboard")}>
                <LayoutDashboard aria-hidden="true" className="h-4 w-4" />{" "}
                Dashboard
              </DropdownMenuItem>
              <DropdownMenuItem onSelect={() => navigate("/change-password")}>
                <KeyRound aria-hidden="true" className="h-4 w-4" /> Change
                Password
              </DropdownMenuItem>
              {hasPermission(Permission.SchoolView) && (
                <DropdownMenuItem onSelect={() => navigate("/settings")}>
                  <Settings aria-hidden="true" className="h-4 w-4" /> School
                  settings
                </DropdownMenuItem>
              )}
              <DropdownMenuSeparator />
              <DropdownMenuItem
                onSelect={handleLogout}
                className="text-destructive focus:text-destructive"
              >
                <LogOut aria-hidden="true" className="h-4 w-4" />
                {loggingOut ? "Signing out…" : "Logout"}
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </header>

        <main className="school-app-canvas min-h-[calc(100vh-4rem)] p-4 sm:p-6 lg:p-8">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
