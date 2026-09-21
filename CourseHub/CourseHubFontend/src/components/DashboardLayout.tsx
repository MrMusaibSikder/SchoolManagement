import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../lib/auth";

interface NavItem {
  to: string;
  label: string;
  /** If set, item is hidden unless the user has this permission (or is SuperAdmin). */
  permission?: string;
}

const NAV_BY_ROLE: Record<"admin" | "teacher" | "student", NavItem[]> = {
  admin: [
    { to: "/admin", label: "Overview" },
    { to: "/admin/courses", label: "Courses", permission: "courses.view" },
    { to: "/admin/teachers", label: "Teachers", permission: "teachers.view" },
    { to: "/admin/students", label: "Students", permission: "students.view" },
    { to: "/admin/batches", label: "Batches", permission: "batches.view" },
    { to: "/admin/enrollments", label: "Enrollments", permission: "enrollments.view" },
    { to: "/admin/assignments", label: "Assignments", permission: "assignments.view" },
    { to: "/admin/users", label: "Users", permission: "users.view" },
    { to: "/admin/roles", label: "Roles & Permissions", permission: "roles.view" },
  ],
  teacher: [
    { to: "/teacher", label: "Overview" },
    { to: "/teacher/courses", label: "Courses", permission: "courses.view" },
    { to: "/teacher/batches", label: "Batches", permission: "batches.view" },
    { to: "/teacher/enrollments", label: "Enrollments", permission: "enrollments.view" },
    { to: "/teacher/assignments", label: "Assignments", permission: "assignments.view" },
  ],
  student: [
    { to: "/student", label: "Overview" },
    { to: "/student/courses", label: "Course catalog" },
    { to: "/student/teachers", label: "Our teachers" },
    { to: "/student/assignments", label: "My assignments" },
    { to: "/student/enrollments", label: "My enrollments" },
  ],
};

const ROLE_LABEL: Record<"admin" | "teacher" | "student", string> = {
  admin: "Admin",
  teacher: "Teacher",
  student: "Student",
};

export function DashboardLayout({ role }: { role: "admin" | "teacher" | "student" }) {
  const { currentUser, logout, hasPermission } = useAuth();
  const navigate = useNavigate();
  const items = NAV_BY_ROLE[role].filter((item) => !item.permission || hasPermission(item.permission));

  async function handleLogout() {
    await logout();
    navigate("/login");
  }

  return (
    <div className="flex min-h-screen">
      <aside className="relative flex w-60 shrink-0 flex-col overflow-hidden bg-gradient-to-b from-slate-900 via-indigo-950 to-slate-900 text-white">
        <div className="pointer-events-none absolute -left-10 -top-10 h-40 w-40 rounded-full bg-brand-500/20 blur-3xl" />
        <div className="pointer-events-none absolute -bottom-16 -right-10 h-40 w-40 rounded-full bg-fuchsia-500/20 blur-3xl" />
        <div className="relative border-b border-white/10 px-5 py-5">
          <div className="flex items-center gap-2">
            <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-gradient-to-br from-brand-400 to-fuchsia-400 text-sm font-bold text-white shadow-lg shadow-brand-500/30">
              C
            </div>
            <p className="font-display text-lg font-bold text-white">CourseHub</p>
          </div>
          <p className="mt-1.5 inline-block rounded-full bg-white/10 px-2 py-0.5 text-xs font-medium text-brand-200">
            {ROLE_LABEL[role]} dashboard
          </p>
        </div>
        <nav className="relative flex-1 space-y-0.5 px-3 py-4">
          {items.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              end={item.to === `/${role}`}
              className={({ isActive }) =>
                `block rounded-lg px-3 py-2 text-sm font-medium transition-all ${
                  isActive
                    ? "bg-gradient-to-r from-brand-500/90 to-fuchsia-500/90 text-white shadow-md shadow-brand-500/20"
                    : "text-slate-300 hover:bg-white/10 hover:text-white"
                }`
              }
            >
              {item.label}
            </NavLink>
          ))}
        </nav>
        <div className="relative border-t border-white/10 p-4">
          <p className="truncate text-sm font-medium text-white">
            {currentUser?.firstName} {currentUser?.lastName}
          </p>
          <p className="truncate text-xs text-slate-400">{currentUser?.email}</p>
          <button
            onClick={handleLogout}
            className="mt-3 w-full rounded-lg border border-white/15 bg-white/5 px-3 py-1.5 text-xs font-medium text-slate-200 transition-colors hover:bg-white/10"
          >
            Log out
          </button>
        </div>
      </aside>
      <main className="flex-1 overflow-x-hidden p-8">
        <Outlet />
      </main>
    </div>
  );
}
