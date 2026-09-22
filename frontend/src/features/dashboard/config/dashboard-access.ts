import { AppRole, Permission } from "@/lib/permissions";

export interface DashboardAccess {
  canViewStudentStats: boolean;
  canViewTeacherStats: boolean;
  canViewEmployeeStats: boolean;
  canViewAttendance: boolean;
  canViewFees: boolean;
  canViewExams: boolean;
  canViewNotices: boolean;
  canViewActivity: boolean;
  hasAnyWidget: boolean;
}

export function buildDashboardAccess(permissions: string[]): DashboardAccess {
  const has = (permission: string) => permissions.includes(permission);

  const access: DashboardAccess = {
    canViewStudentStats: has(Permission.StudentView),
    canViewTeacherStats: has(Permission.TeacherView),
    canViewEmployeeStats: has(Permission.EmployeeView),
    canViewAttendance: has(Permission.AttendanceReportView),
    canViewFees: has(Permission.InvoiceView),
    canViewExams: has(Permission.ExamView),
    canViewNotices: has(Permission.NoticeView),
    canViewActivity: has(Permission.StudentView) || has(Permission.InvoiceView),
    hasAnyWidget: false,
  };

  access.hasAnyWidget =
    access.canViewStudentStats ||
    access.canViewTeacherStats ||
    access.canViewEmployeeStats ||
    access.canViewAttendance ||
    access.canViewFees ||
    access.canViewExams ||
    access.canViewNotices ||
    access.canViewActivity;

  return access;
}

export interface QuickActionItem {
  label: string;
  description: string;
  to: string;
  permission: string;
}

export const QUICK_ACTIONS: QuickActionItem[] = [
  {
    label: "Add Student",
    description: "Register a new learner",
    to: "/students/new",
    permission: Permission.StudentCreate,
  },
  {
    label: "Add Guardian",
    description: "Create a guardian profile",
    to: "/guardians/new",
    permission: Permission.GuardianCreate,
  },
  {
    label: "Add Employee",
    description: "Create a staff profile with photo",
    to: "/employees/new",
    permission: Permission.EmployeeCreate,
  },
  {
    label: "View Employees",
    description: "Browse staff records",
    to: "/employees",
    permission: Permission.EmployeeView,
  },
  {
    label: "Take attendance",
    description: "Mark student or staff attendance",
    to: "/attendance",
    permission: Permission.StudentAttendanceCreate,
  },
  {
    label: "Create exam",
    description: "Open the exam board",
    to: "/exams/list",
    permission: Permission.ExamCreate,
  },
  {
    label: "Academic Setup",
    description: "Manage years, classes, and subjects",
    to: "/academic",
    permission: Permission.AcademicYearView,
  },
  {
    label: "View Students",
    description: "Browse the student directory",
    to: "/students",
    permission: Permission.StudentView,
  },
  {
    label: "View Guardians",
    description: "Browse guardian records",
    to: "/guardians",
    permission: Permission.GuardianView,
  },
];

export function getDashboardWelcome(primaryRole: string) {
  switch (primaryRole) {
    case AppRole.Admin:
      return {
        eyebrow: "School overview",
        subtitle:
          "Review student growth, staffing, fee activity, exams, and notices from one place.",
      };
    case AppRole.Teacher:
      return {
        eyebrow: "Teacher dashboard",
        subtitle:
          "Focus on attendance, upcoming exams, and school notices relevant to your classes.",
      };
    case AppRole.Accountant:
      return {
        eyebrow: "Finance dashboard",
        subtitle:
          "Track fee collection, pending balances, and recent invoice activity.",
      };
    case AppRole.Student:
      return {
        eyebrow: "Student portal",
        subtitle:
          "See notices, academic updates, and information available to you.",
      };
    default:
      return {
        eyebrow: "Dashboard",
        subtitle: "Your workspace shows only the modules you are allowed to access.",
      };
  }
}
