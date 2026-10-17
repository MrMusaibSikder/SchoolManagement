import { Link } from "react-router-dom";
import {
  ArrowRight,
  BarChart3,
  Briefcase,
  CheckCircle2,
  ClipboardList,
  GraduationCap,
} from "lucide-react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { PermissionGuard } from "@/features/auth/components/PermissionGuard";
import { Permission } from "@/lib/permissions";

const AREAS = [
  {
    to: "/attendance/students",
    title: "Student attendance",
    description: "Mark daily attendance by class and section. Existing entries are updated for the same date.",
    icon: GraduationCap,
    color: "border-t-emerald-500",
    iconColor: "text-emerald-700 bg-emerald-50 group-hover:bg-emerald-600 group-hover:text-white dark:bg-emerald-950/60 dark:text-emerald-300",
    anyOf: [Permission.StudentAttendanceView, Permission.StudentAttendanceCreate],
  },
  {
    to: "/attendance/employees",
    title: "Employee attendance",
    description: "Mark staff attendance for a date, including optional check-in and check-out times.",
    icon: Briefcase,
    color: "border-t-sky-500",
    iconColor: "text-sky-700 bg-sky-50 group-hover:bg-sky-600 group-hover:text-white dark:bg-sky-950/60 dark:text-sky-300",
    anyOf: [Permission.EmployeeAttendanceView, Permission.EmployeeAttendanceCreate],
  },
  {
    to: "/attendance/reports",
    title: "Attendance reports",
    description: "Today’s summary, class totals, date-range overview, and attendance trend.",
    icon: BarChart3,
    color: "border-t-amber-400",
    iconColor: "text-amber-800 bg-amber-50 group-hover:bg-amber-500 group-hover:text-white dark:bg-amber-950/50 dark:text-amber-300",
    permission: Permission.AttendanceReportView,
  },
] as const;

export function AttendanceHubPage() {
  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <Card className="overflow-hidden border-primary/20 bg-gradient-to-br from-primary to-primary/90 text-primary-foreground">
        <CardContent className="flex flex-col gap-4 p-6 sm:flex-row sm:items-center sm:justify-between sm:p-8">
          <div>
            <p className="text-xs font-semibold uppercase tracking-[0.2em] text-primary-foreground/70">
              Daily school operations
            </p>
            <h1 className="mt-2 font-display text-2xl font-semibold sm:text-3xl">
              Attendance management
            </h1>
            <p className="mt-2 max-w-2xl text-sm leading-6 text-primary-foreground/80">
              Record daily attendance for students and staff, then review
              attendance summaries and trends.
            </p>
          </div>
          <div className="flex h-14 w-14 shrink-0 items-center justify-center self-start rounded-2xl border border-white/15 bg-white/10 sm:self-center">
            <ClipboardList aria-hidden="true" className="h-7 w-7" />
          </div>
        </CardContent>
      </Card>

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
        {AREAS.map((area) => (
          <PermissionGuard
            key={area.to}
            permission={"permission" in area ? area.permission : undefined}
            anyOf={"anyOf" in area ? [...area.anyOf] : undefined}
          >
            <Link to={area.to} className="group block h-full rounded-lg focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring">
              <Card className={`h-full border-t-4 transition duration-200 group-hover:-translate-y-0.5 group-hover:shadow-md ${area.color}`}>
                <CardContent className="flex h-full flex-col p-5">
                  <span className={`mb-4 flex h-11 w-11 items-center justify-center rounded-xl transition-colors ${area.iconColor}`}>
                    <area.icon className="h-5 w-5" aria-hidden="true" />
                  </span>
                  <CardTitle className="text-base">{area.title}</CardTitle>
                  <CardDescription className="mt-2 flex-1 leading-5">
                    {area.description}
                  </CardDescription>
                  <span className="mt-5 inline-flex items-center gap-2 text-sm font-semibold text-primary">
                    Open section
                    <ArrowRight aria-hidden="true" className="h-4 w-4 transition-transform group-hover:translate-x-1" />
                  </span>
                </CardContent>
              </Card>
            </Link>
          </PermissionGuard>
        ))}
      </div>

      <Card className="border-dashed bg-card/80">
        <CardHeader className="pb-3">
          <CardTitle className="flex items-center gap-2 text-base">
            <ClipboardList className="h-5 w-5 text-primary" aria-hidden="true" />
            Attendance rules
          </CardTitle>
          <CardDescription>
            A few important rules when recording daily attendance.
          </CardDescription>
        </CardHeader>
        <CardContent className="grid gap-3 text-sm sm:grid-cols-3">
          {[
            "Future dates cannot be marked.",
            "Only one record per person is kept each day.",
            "Saving again updates that day’s existing records.",
          ].map((rule) => (
            <div key={rule} className="flex items-start gap-2 rounded-lg border bg-background/80 p-3">
              <CheckCircle2 aria-hidden="true" className="mt-0.5 h-4 w-4 shrink-0 text-emerald-600" />
              <p className="text-muted-foreground">{rule}</p>
            </div>
          ))}
        </CardContent>
      </Card>
    </div>
  );
}
