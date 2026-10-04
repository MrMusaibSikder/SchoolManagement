import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2 } from "lucide-react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { useSchoolClasses, useSections } from "@/features/academic/hooks/useAcademicData";
import { useEmployees } from "@/features/employee/hooks/useEmployeeData";
import type { EmployeeDto } from "@/features/employee/types/employee.types";
import { getApiErrorMessage } from "@/lib/api/error-message";
import { Permission } from "@/lib/permissions";
import {
  useAdminAttendanceDashboard,
  useAttendanceTrend,
  useClassAttendanceSummary,
  useEmployeeAttendanceHistory,
  useTodayAttendanceDashboard,
} from "../hooks/useAttendanceData";
import {
  AttendanceStatus,
  attendanceStatusLabel,
  todayIsoDate,
} from "../types/attendance.types";

const EMPTY_EMPLOYEES: EmployeeDto[] = [];

function daysAgo(days: number) {
  const date = new Date();
  date.setDate(date.getDate() - days);
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${date.getFullYear()}-${month}-${day}`;
}

export function AttendanceReportsPage() {
  const { hasPermission } = usePermissions();
  const canViewEmployeeAttendance = hasPermission(Permission.EmployeeAttendanceView);
  const canViewEmployees = hasPermission(Permission.EmployeeView);
  const { data: classes = [] } = useSchoolClasses();
  const { data: sections = [] } = useSections();
  const employeesQuery = useEmployees(canViewEmployees);
  const employees = employeesQuery.data ?? EMPTY_EMPLOYEES;
  const today = todayIsoDate();
  const [classId, setClassId] = useState("");
  const [sectionId, setSectionId] = useState("");
  const [classDate, setClassDate] = useState(today);
  const [fromDate, setFromDate] = useState(daysAgo(7));
  const [toDate, setToDate] = useState(today);
  const [employeeId, setEmployeeId] = useState("");
  const [employeeSearch, setEmployeeSearch] = useState("");
  const [employeeFromDate, setEmployeeFromDate] = useState(daysAgo(7));
  const [employeeToDate, setEmployeeToDate] = useState(today);

  const classSections = useMemo(
    () => sections.filter((item) => String(item.classId) === classId),
    [sections, classId]
  );

  const todayReport = useTodayAttendanceDashboard(true);
  const classReport = useClassAttendanceSummary(
    {
      classId: classId ? Number(classId) : null,
      sectionId: sectionId ? Number(sectionId) : null,
      attendanceDate: classDate,
    },
    Boolean(classId && sectionId)
  );
  const adminReport = useAdminAttendanceDashboard({ fromDate, toDate }, true);
  const trend = useAttendanceTrend({ fromDate, toDate }, true);
  const employeeHistory = useEmployeeAttendanceHistory(
    {
      employeeId: employeeId ? Number(employeeId) : null,
      fromDate: employeeFromDate,
      toDate: employeeToDate,
    },
    canViewEmployeeAttendance && canViewEmployees
  );
  const filteredEmployees = useMemo(() => {
    const search = employeeSearch.trim().toLocaleLowerCase();
    return employees
      .filter((employee) =>
        `${employee.fullName} ${employee.employeeCode}`
          .toLocaleLowerCase()
          .includes(search)
      )
      .sort((left, right) => left.fullName.localeCompare(right.fullName));
  }, [employeeSearch, employees]);
  const selectedEmployee = employees.find(
    (employee) => String(employee.id) === employeeId
  );
  const employeeRecords = useMemo(
    () => employeeHistory.data ?? [],
    [employeeHistory.data]
  );
  const employeeSummary = useMemo(
    () => ({
      total: employeeRecords.length,
      present: employeeRecords.filter(
        (record) => record.status === AttendanceStatus.Present
      ).length,
      absent: employeeRecords.filter(
        (record) => record.status === AttendanceStatus.Absent
      ).length,
      late: employeeRecords.filter(
        (record) => record.status === AttendanceStatus.Late
      ).length,
      leave: employeeRecords.filter(
        (record) => record.status === AttendanceStatus.Leave
      ).length,
      halfDay: employeeRecords.filter(
        (record) => record.status === AttendanceStatus.HalfDay
      ).length,
    }),
    [employeeRecords]
  );

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Attendance reports</h1>
          <p className="text-sm text-muted-foreground">
            Read-only summaries from the attendance report APIs.
          </p>
        </div>
        <Link
          to="/attendance"
          className="inline-flex items-center justify-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent"
        >
          Back
        </Link>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Today</CardTitle>
          <CardDescription>School-wide student attendance for today.</CardDescription>
        </CardHeader>
        <CardContent>
          {todayReport.isPending ? (
            <div className="flex items-center text-sm text-muted-foreground">
              <Loader2 className="mr-2 h-4 w-4 animate-spin" /> Loading…
            </div>
          ) : todayReport.isError || !todayReport.data ? (
            <p className="text-sm text-destructive">Unable to load today’s summary.</p>
          ) : (
            <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-5">
              <Stat label="Students" value={todayReport.data.totalStudents} />
              <Stat label="Present" value={todayReport.data.presentToday} />
              <Stat label="Absent" value={todayReport.data.absentToday} />
              <Stat label="Late" value={todayReport.data.lateToday} />
              <Stat
                label="Rate"
                value={`${todayReport.data.attendancePercentage.toFixed(1)}%`}
              />
            </div>
          )}
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Class summary</CardTitle>
          <CardDescription>Totals for one class, section, and date.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 md:grid-cols-3">
            <select
              value={classId}
              onChange={(event) => {
                setClassId(event.target.value);
                setSectionId("");
              }}
              className="rounded-md border bg-background px-3 py-2"
            >
              <option value="">Select class</option>
              {classes.map((item) => (
                <option key={item.id} value={item.id}>
                  {item.name}
                </option>
              ))}
            </select>
            <select
              value={sectionId}
              onChange={(event) => setSectionId(event.target.value)}
              disabled={!classId}
              className="rounded-md border bg-background px-3 py-2"
            >
              <option value="">Select section</option>
              {classSections.map((item) => (
                <option key={item.id} value={item.id}>
                  {item.name}
                </option>
              ))}
            </select>
            <input
              type="date"
              max={today}
              value={classDate}
              onChange={(event) => setClassDate(event.target.value)}
              className="rounded-md border bg-background px-3 py-2"
            />
          </div>
          {!classId || !sectionId ? (
            <p className="text-sm text-muted-foreground">Select a class and section to load the summary.</p>
          ) : classReport.isPending ? (
            <div className="flex items-center text-sm text-muted-foreground">
              <Loader2 className="mr-2 h-4 w-4 animate-spin" /> Loading…
            </div>
          ) : classReport.isError || !classReport.data ? (
            <p className="text-sm text-destructive">Unable to load class summary.</p>
          ) : (
            <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-5">
              <Stat label="Students" value={classReport.data.totalStudents} />
              <Stat label="Present" value={classReport.data.present} />
              <Stat label="Absent" value={classReport.data.absent} />
              <Stat label="Late" value={classReport.data.late} />
              <Stat label="Rate" value={`${classReport.data.percentage.toFixed(1)}%`} />
            </div>
          )}
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Date range</CardTitle>
          <CardDescription>
            Student dashboard totals and daily trend for the selected date
            range.
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 md:grid-cols-2">
            <input
              type="date"
              max={toDate || today}
              value={fromDate}
              onChange={(event) => setFromDate(event.target.value)}
              className="rounded-md border bg-background px-3 py-2"
            />
            <input
              type="date"
              max={today}
              value={toDate}
              onChange={(event) => setToDate(event.target.value)}
              className="rounded-md border bg-background px-3 py-2"
            />
          </div>
          {adminReport.isPending ? (
            <div className="flex items-center text-sm text-muted-foreground">
              <Loader2 className="mr-2 h-4 w-4 animate-spin" /> Loading…
            </div>
          ) : adminReport.isError || !adminReport.data ? (
            <p className="text-sm text-destructive">Unable to load the date-range summary.</p>
          ) : (
            <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-5">
              <Stat label="Students" value={adminReport.data.totalStudents} />
              <Stat label="Present" value={adminReport.data.totalPresent} />
              <Stat label="Absent" value={adminReport.data.totalAbsent} />
              <Stat label="Late" value={adminReport.data.totalLate} />
              <Stat
                label="Rate"
                value={`${adminReport.data.attendancePercentage.toFixed(1)}%`}
              />
            </div>
          )}

          {trend.isPending ? null : trend.isError ? (
            <p className="text-sm text-destructive">Unable to load trend data.</p>
          ) : (trend.data ?? []).length === 0 ? (
            <p className="text-sm text-muted-foreground">No trend data for this range.</p>
          ) : (
            <div className="overflow-x-auto rounded-lg border">
              <table className="w-full min-w-[520px] text-left text-sm">
                <thead className="border-b bg-muted/40">
                  <tr>
                    <th className="px-3 py-2 font-medium">Date</th>
                    <th className="px-3 py-2 font-medium">Present</th>
                    <th className="px-3 py-2 font-medium">Absent</th>
                    <th className="px-3 py-2 font-medium">Late</th>
                    <th className="px-3 py-2 font-medium">Leave</th>
                  </tr>
                </thead>
                <tbody>
                  {(trend.data ?? []).map((item) => (
                    <tr key={item.date} className="border-b last:border-0">
                      <td className="px-3 py-2">
                        {new Date(item.date).toLocaleDateString("en-BD")}
                      </td>
                      <td className="px-3 py-2">{item.present}</td>
                      <td className="px-3 py-2">{item.absent}</td>
                      <td className="px-3 py-2">{item.late}</td>
                      <td className="px-3 py-2">{item.leave}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Employee attendance report</CardTitle>
          <CardDescription>
            Review an employee&apos;s attendance, check-in and check-out history
            for the selected date range.
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          {!canViewEmployeeAttendance ? (
            <p className="rounded-lg border border-amber-500/30 bg-amber-500/5 p-4 text-sm text-muted-foreground">
              Employee attendance history requires Employee Attendance View
              permission.
            </p>
          ) : !canViewEmployees ? (
            <p className="rounded-lg border border-amber-500/30 bg-amber-500/5 p-4 text-sm text-muted-foreground">
              Employee name selection requires Employee View permission.
            </p>
          ) : employeesQuery.isPending ? (
            <div className="flex items-center text-sm text-muted-foreground">
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              Loading employees…
            </div>
          ) : employeesQuery.isError ? (
            <div className="flex flex-col gap-3 rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive sm:flex-row sm:items-center sm:justify-between">
              <p>
                {getApiErrorMessage(
                  employeesQuery.error,
                  "Unable to load employees for the report."
                )}
              </p>
              <button
                type="button"
                onClick={() => void employeesQuery.refetch()}
                className="rounded-md border px-3 py-1.5 font-medium hover:bg-background"
              >
                Try again
              </button>
            </div>
          ) : (
            <>
              <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
                <div className="space-y-1.5">
                  <label htmlFor="employee-report-search" className="text-sm font-medium">
                    Find employee
                  </label>
                  <input
                    id="employee-report-search"
                    type="search"
                    value={employeeSearch}
                    onChange={(event) => {
                      setEmployeeSearch(event.target.value);
                      setEmployeeId("");
                    }}
                    placeholder="Search by name or employee code"
                    className="w-full rounded-md border bg-background px-3 py-2"
                  />
                </div>
                <div className="space-y-1.5">
                  <label htmlFor="employee-report-id" className="text-sm font-medium">
                    Employee
                  </label>
                  <select
                    id="employee-report-id"
                    value={employeeId}
                    onChange={(event) => setEmployeeId(event.target.value)}
                    disabled={filteredEmployees.length === 0}
                    className="w-full rounded-md border bg-background px-3 py-2 disabled:opacity-60"
                  >
                    <option value="">Select employee</option>
                    {filteredEmployees.map((employee) => (
                      <option key={employee.id} value={employee.id}>
                        {employee.fullName} · {employee.employeeCode}
                      </option>
                    ))}
                  </select>
                  {filteredEmployees.length === 0 && (
                    <p className="text-xs text-muted-foreground">
                      No employees match this search.
                    </p>
                  )}
                </div>
                <div className="space-y-1.5">
                  <label htmlFor="employee-report-from" className="text-sm font-medium">
                    From
                  </label>
                  <input
                    id="employee-report-from"
                    type="date"
                    max={employeeToDate || today}
                    value={employeeFromDate}
                    onChange={(event) =>
                      setEmployeeFromDate(event.target.value)
                    }
                    className="w-full rounded-md border bg-background px-3 py-2"
                  />
                </div>
                <div className="space-y-1.5">
                  <label htmlFor="employee-report-to" className="text-sm font-medium">
                    To
                  </label>
                  <input
                    id="employee-report-to"
                    type="date"
                    min={employeeFromDate}
                    max={today}
                    value={employeeToDate}
                    onChange={(event) => setEmployeeToDate(event.target.value)}
                    className="w-full rounded-md border bg-background px-3 py-2"
                  />
                </div>
              </div>

              {!employeeId ? (
                <p className="rounded-lg border border-dashed p-6 text-center text-sm text-muted-foreground">
                  Search for and select an employee to view their attendance
                  report.
                </p>
              ) : employeeFromDate > employeeToDate ? (
                <p className="text-sm text-destructive">
                  The end date must be on or after the start date.
                </p>
              ) : employeeHistory.isPending ? (
                <div className="flex items-center text-sm text-muted-foreground">
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Loading {selectedEmployee?.fullName ?? "employee"} attendance…
                </div>
              ) : employeeHistory.isError ? (
                <div className="flex flex-col gap-3 rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive sm:flex-row sm:items-center sm:justify-between">
                  <p>
                    {getApiErrorMessage(
                      employeeHistory.error,
                      "Unable to load employee attendance history."
                    )}
                  </p>
                  <button
                    type="button"
                    onClick={() => void employeeHistory.refetch()}
                    className="rounded-md border px-3 py-1.5 font-medium hover:bg-background"
                  >
                    Try again
                  </button>
                </div>
              ) : (
                <>
                  <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-6">
                    <Stat label="Recorded days" value={employeeSummary.total} />
                    <Stat label="Present" value={employeeSummary.present} />
                    <Stat label="Absent" value={employeeSummary.absent} />
                    <Stat label="Late" value={employeeSummary.late} />
                    <Stat label="Leave" value={employeeSummary.leave} />
                    <Stat label="Half day" value={employeeSummary.halfDay} />
                  </div>

                  {employeeRecords.length === 0 ? (
                    <p className="rounded-lg border border-dashed p-6 text-center text-sm text-muted-foreground">
                      No attendance records found for{" "}
                      {selectedEmployee?.fullName ?? "this employee"} in the
                      selected date range.
                    </p>
                  ) : (
                    <div className="overflow-x-auto rounded-lg border">
                      <table className="w-full min-w-[680px] text-left text-sm">
                        <thead className="border-b bg-muted/40">
                          <tr>
                            <th className="px-3 py-2 font-medium">Date</th>
                            <th className="px-3 py-2 font-medium">Status</th>
                            <th className="px-3 py-2 font-medium">Check in</th>
                            <th className="px-3 py-2 font-medium">Check out</th>
                          </tr>
                        </thead>
                        <tbody>
                          {employeeRecords.map((record) => (
                            <tr
                              key={record.id}
                              className="border-b last:border-0"
                            >
                              <td className="px-3 py-2">
                                {formatAttendanceDate(record.attendanceDate)}
                              </td>
                              <td className="px-3 py-2">
                                {attendanceStatusLabel(record.status)}
                              </td>
                              <td className="px-3 py-2 tabular-nums">
                                {formatAttendanceDateTime(record.checkIn)}
                              </td>
                              <td className="px-3 py-2 tabular-nums">
                                {formatAttendanceDateTime(record.checkOut)}
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}
                </>
              )}
            </>
          )}
        </CardContent>
      </Card>
    </div>
  );
}

function formatAttendanceDate(value: string) {
  const date = value.slice(0, 10);
  const localDate = new Date(`${date}T00:00:00`);
  return Number.isNaN(localDate.getTime())
    ? date
    : localDate.toLocaleDateString("en-BD", {
        day: "numeric",
        month: "short",
        year: "numeric",
      });
}

function formatAttendanceDateTime(value?: string | null) {
  if (!value) return "—";
  const dateTime = new Date(value);
  return Number.isNaN(dateTime.getTime())
    ? value
    : dateTime.toLocaleString("en-BD", {
        day: "numeric",
        month: "short",
        year: "numeric",
        hour: "numeric",
        minute: "2-digit",
      });
}

function Stat({ label, value }: { label: string; value: string | number }) {
  return (
    <div className="rounded-lg border bg-muted/30 p-3">
      <p className="text-xs text-muted-foreground">{label}</p>
      <p className="mt-1 text-xl font-semibold tabular-nums">{value}</p>
    </div>
  );
}
