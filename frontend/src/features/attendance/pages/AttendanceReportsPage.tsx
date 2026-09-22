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
import { useSchoolClasses, useSections } from "@/features/academic/hooks/useAcademicData";
import {
  useAdminAttendanceDashboard,
  useAttendanceTrend,
  useClassAttendanceSummary,
  useTodayAttendanceDashboard,
} from "../hooks/useAttendanceData";
import { todayIsoDate } from "../types/attendance.types";

function daysAgo(days: number) {
  const date = new Date();
  date.setDate(date.getDate() - days);
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${date.getFullYear()}-${month}-${day}`;
}

export function AttendanceReportsPage() {
  const { data: classes = [] } = useSchoolClasses();
  const { data: sections = [] } = useSections();
  const today = todayIsoDate();
  const [classId, setClassId] = useState("");
  const [sectionId, setSectionId] = useState("");
  const [classDate, setClassDate] = useState(today);
  const [fromDate, setFromDate] = useState(daysAgo(7));
  const [toDate, setToDate] = useState(today);

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
          <CardDescription>Admin dashboard totals and daily trend.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 md:grid-cols-2">
            <input
              type="date"
              max={today}
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
    </div>
  );
}

function Stat({ label, value }: { label: string; value: string | number }) {
  return (
    <div className="rounded-lg border bg-muted/30 p-3">
      <p className="text-xs text-muted-foreground">{label}</p>
      <p className="mt-1 text-xl font-semibold tabular-nums">{value}</p>
    </div>
  );
}
