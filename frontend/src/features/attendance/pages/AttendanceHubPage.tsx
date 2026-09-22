import { Link } from "react-router-dom";
import { ClipboardList, GraduationCap, Briefcase, BarChart3 } from "lucide-react";
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
    anyOf: [Permission.StudentAttendanceView, Permission.StudentAttendanceCreate],
  },
  {
    to: "/attendance/employees",
    title: "Employee attendance",
    description: "Mark staff attendance for a date, including optional check-in and check-out times.",
    icon: Briefcase,
    anyOf: [Permission.EmployeeAttendanceView, Permission.EmployeeAttendanceCreate],
  },
  {
    to: "/attendance/reports",
    title: "Attendance reports",
    description: "Today’s summary, class totals, date-range overview, and attendance trend.",
    icon: BarChart3,
    permission: Permission.AttendanceReportView,
  },
] as const;

export function AttendanceHubPage() {
  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div>
        <h1 className="font-display text-2xl font-semibold">Attendance</h1>
        <p className="text-sm text-muted-foreground">
          Take daily attendance once per class or staff list. Duplicate days are saved as updates, not extra records.
        </p>
      </div>

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
        {AREAS.map((area) => (
          <PermissionGuard
            key={area.to}
            permission={"permission" in area ? area.permission : undefined}
            anyOf={"anyOf" in area ? [...area.anyOf] : undefined}
          >
            <Link to={area.to} className="block h-full">
              <Card className="h-full transition hover:border-primary/40 hover:bg-muted/30">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2 text-lg">
                    <area.icon className="h-5 w-5 text-muted-foreground" aria-hidden="true" />
                    {area.title}
                  </CardTitle>
                  <CardDescription>{area.description}</CardDescription>
                </CardHeader>
                <CardContent className="text-sm font-medium text-primary">Open</CardContent>
              </Card>
            </Link>
          </PermissionGuard>
        ))}
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <ClipboardList className="h-5 w-5 text-muted-foreground" />
            Rules
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-1 text-sm text-muted-foreground">
          <p>Future dates cannot be marked.</p>
          <p>Each student or employee can have only one record per day.</p>
          <p>Saving again on the same date updates the existing record.</p>
        </CardContent>
      </Card>
    </div>
  );
}
