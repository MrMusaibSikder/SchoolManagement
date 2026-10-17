import {
  BookOpenCheck,
  BriefcaseBusiness,
  ClipboardCheck,
  FilePlus2,
  GraduationCap,
  HeartHandshake,
  ShieldCheck,
  UserPlus,
  Users,
  UsersRound,
  type LucideIcon,
} from "lucide-react";
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
  icon: LucideIcon;
  color: "green" | "gold" | "blue" | "violet" | "teal" | "rose";
  permission?: string;
  adminOnly?: boolean;
}

export const QUICK_ACTIONS: QuickActionItem[] = [
  {
    label: "Add Student",
    description: "Register a new learner",
    to: "/students/new",
    icon: GraduationCap,
    color: "green",
    permission: Permission.StudentCreate,
  },
  {
    label: "Add Guardian",
    description: "Create a guardian profile",
    to: "/guardians/new",
    icon: HeartHandshake,
    color: "gold",
    permission: Permission.GuardianCreate,
  },
  {
    label: "Add Employee",
    description: "Create a staff profile with photo",
    to: "/employees/new",
    icon: UserPlus,
    color: "blue",
    permission: Permission.EmployeeCreate,
  },
  {
    label: "View Employees",
    description: "Browse staff records",
    to: "/employees",
    icon: BriefcaseBusiness,
    color: "violet",
    permission: Permission.EmployeeView,
  },
  {
    label: "Take attendance",
    description: "Mark student or staff attendance",
    to: "/attendance",
    icon: ClipboardCheck,
    color: "teal",
    permission: Permission.StudentAttendanceCreate,
  },
  {
    label: "Create exam",
    description: "Open the exam board",
    to: "/exams/list",
    icon: FilePlus2,
    color: "rose",
    permission: Permission.ExamCreate,
  },
  {
    label: "Roles & Access",
    description: "Assign roles and review inherited user access",
    to: "/roles",
    icon: ShieldCheck,
    color: "violet",
    adminOnly: true,
  },
  {
    label: "Academic Setup",
    description: "Manage years, classes, and subjects",
    to: "/academic",
    icon: BookOpenCheck,
    color: "blue",
    permission: Permission.AcademicYearView,
  },
  {
    label: "View Students",
    description: "Browse the student directory",
    to: "/students",
    icon: Users,
    color: "green",
    permission: Permission.StudentView,
  },
  {
    label: "View Guardians",
    description: "Browse guardian records",
    to: "/guardians",
    icon: UsersRound,
    color: "gold",
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
