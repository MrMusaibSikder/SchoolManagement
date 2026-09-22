import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { useSchoolClasses, useSections } from "@/features/academic/hooks/useAcademicData";
import { useStudents } from "@/features/student/hooks/useStudentData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import {
  useBulkStudentAttendance,
  useStudentAttendanceSheet,
} from "../hooks/useAttendanceData";
import {
  ATTENDANCE_STATUS_OPTIONS,
  AttendanceStatus,
  todayIsoDate,
  type AttendanceStatusValue,
} from "../types/attendance.types";

interface RowState {
  studentId: number;
  fullName: string;
  rollNo: string;
  status: AttendanceStatusValue;
  remarks: string;
}

export function StudentAttendancePage() {
  const { hasPermission } = usePermissions();
  const canSave = hasPermission(Permission.StudentAttendanceCreate);
  const { data: classes = [] } = useSchoolClasses();
  const { data: sections = [] } = useSections();
  const { data: students = [], isPending: studentsPending } = useStudents();
  const saveMutation = useBulkStudentAttendance();

  const [classId, setClassId] = useState("");
  const [sectionId, setSectionId] = useState("");
  const [attendanceDate, setAttendanceDate] = useState(todayIsoDate());
  const [rows, setRows] = useState<RowState[]>([]);

  const numericClassId = classId ? Number(classId) : null;
  const numericSectionId = sectionId ? Number(sectionId) : null;

  const classSections = useMemo(
    () => sections.filter((item) => String(item.classId) === classId),
    [sections, classId]
  );

  const roster = useMemo(
    () =>
      students.filter(
        (item) =>
          numericClassId != null &&
          numericSectionId != null &&
          item.classId === numericClassId &&
          item.sectionId === numericSectionId
      ),
    [students, numericClassId, numericSectionId]
  );

  const sheet = useStudentAttendanceSheet({
    classId: numericClassId,
    sectionId: numericSectionId,
    attendanceDate,
  });

  useEffect(() => {
    if (!numericClassId || !numericSectionId) {
      setRows([]);
      return;
    }
    const existing = new Map((sheet.data ?? []).map((item) => [item.studentId, item]));
    setRows(
      roster.map((student) => {
        const record = existing.get(student.id);
        return {
          studentId: student.id,
          fullName: student.fullName,
          rollNo: student.rollNo,
          status: record?.status ?? AttendanceStatus.Present,
          remarks: record?.remarks ?? "",
        };
      })
    );
  }, [roster, sheet.data, numericClassId, numericSectionId]);

  function updateRow(studentId: number, patch: Partial<RowState>) {
    setRows((current) =>
      current.map((row) => (row.studentId === studentId ? { ...row, ...patch } : row))
    );
  }

  async function handleSave() {
    if (!numericClassId || !numericSectionId || rows.length === 0) return;
    try {
      await saveMutation.mutateAsync({
        classId: numericClassId,
        sectionId: numericSectionId,
        attendanceDate,
        attendance: rows.map((row) => ({
          studentId: row.studentId,
          status: row.status,
          remarks: row.remarks.trim() || null,
        })),
      });
      toast.success("Student attendance saved.");
    } catch {
      toast.error("Could not save attendance. Future dates and empty lists are not allowed.");
    }
  }

  const loading = studentsPending || sheet.isFetching;

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Student attendance</h1>
          <p className="text-sm text-muted-foreground">
            Choose class, section, and date. Saving updates existing records for that day.
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
          <CardTitle>Register</CardTitle>
          <CardDescription>Attendance cannot be marked for a future date.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 md:grid-cols-4">
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
              className="rounded-md border bg-background px-3 py-2"
              disabled={!classId}
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
              max={todayIsoDate()}
              value={attendanceDate}
              onChange={(event) => setAttendanceDate(event.target.value)}
              className="rounded-md border bg-background px-3 py-2"
            />
            <div className="flex items-center gap-2">
              {canSave ? (
                <>
                  <Button
                    type="button"
                    variant="outline"
                    disabled={rows.length === 0}
                    onClick={() =>
                      setRows((current) =>
                        current.map((row) => ({ ...row, status: AttendanceStatus.Present }))
                      )
                    }
                  >
                    All present
                  </Button>
                  <Button
                    type="button"
                    disabled={saveMutation.isPending || rows.length === 0}
                    onClick={() => void handleSave()}
                  >
                    {saveMutation.isPending ? (
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    ) : null}
                    Save
                  </Button>
                </>
              ) : null}
            </div>
          </div>

          {loading ? (
            <div className="flex items-center justify-center py-10 text-sm text-muted-foreground">
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              Loading register…
            </div>
          ) : null}

          {sheet.isError ? (
            <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">
              Unable to load existing attendance for this date.
            </div>
          ) : null}

          {!loading && numericClassId && numericSectionId && rows.length === 0 ? (
            <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">
              No students found in this class and section.
            </div>
          ) : null}

          {rows.length > 0 ? (
            <div className="overflow-x-auto rounded-lg border">
              <table className="w-full min-w-[640px] text-left text-sm">
                <thead className="border-b bg-muted/40">
                  <tr>
                    <th className="px-3 py-2 font-medium">Student</th>
                    <th className="px-3 py-2 font-medium">Roll</th>
                    <th className="px-3 py-2 font-medium">Status</th>
                    <th className="px-3 py-2 font-medium">Remarks</th>
                  </tr>
                </thead>
                <tbody>
                  {rows.map((row) => (
                    <tr key={row.studentId} className="border-b last:border-0">
                      <td className="px-3 py-2 font-medium">{row.fullName}</td>
                      <td className="px-3 py-2 text-muted-foreground">{row.rollNo}</td>
                      <td className="px-3 py-2">
                        <select
                          value={row.status}
                          disabled={!canSave}
                          onChange={(event) =>
                            updateRow(row.studentId, {
                              status: Number(event.target.value) as AttendanceStatusValue,
                            })
                          }
                          className="w-full rounded-md border bg-background px-2 py-1.5"
                        >
                          {ATTENDANCE_STATUS_OPTIONS.map((option) => (
                            <option key={option.value} value={option.value}>
                              {option.label}
                            </option>
                          ))}
                        </select>
                      </td>
                      <td className="px-3 py-2">
                        <input
                          value={row.remarks}
                          disabled={!canSave}
                          onChange={(event) =>
                            updateRow(row.studentId, { remarks: event.target.value })
                          }
                          className="w-full rounded-md border bg-background px-2 py-1.5"
                          maxLength={200}
                        />
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : null}
        </CardContent>
      </Card>
    </div>
  );
}
