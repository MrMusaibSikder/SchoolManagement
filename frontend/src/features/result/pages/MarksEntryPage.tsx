import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, Lock, Save, Send, Unlock } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { useExams, useExamDetails } from "@/features/exam/hooks/useExamData";
import { ExamStatus } from "@/features/exam/types/exam.types";
import {
  useStudents,
  useSubjectTeachers,
  useTeachers,
} from "@/features/academic/hooks/useAcademicData";
import { useEmployees } from "@/features/employee/hooks/useEmployeeData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { getResultErrorMessage } from "../api/result.api";
import {
  useMarkEntries,
  useMarkEntryLock,
  useSaveBulkMarkEntries,
  useSubmitMarkEntries,
} from "../hooks/useResultData";
import {
  MarkAttendanceStatus,
  MarkEntryStatus,
  type MarkAttendanceStatusValue,
} from "../types/result.types";

type Draft = {
  marksObtained: string;
  graceMarks: string;
  attendanceStatus: MarkAttendanceStatusValue;
  remarks: string;
};

const EMPTY_LIST: never[] = [];

const attendanceOptions: Array<[MarkAttendanceStatusValue, string]> = [
  [MarkAttendanceStatus.Present, "Present"],
  [MarkAttendanceStatus.Absent, "Absent"],
  [MarkAttendanceStatus.Medical, "Medical"],
  [MarkAttendanceStatus.Withheld, "Withheld"],
  [MarkAttendanceStatus.Incomplete, "Incomplete"],
  [MarkAttendanceStatus.Excused, "Excused"],
  [MarkAttendanceStatus.Late, "Late"],
  [MarkAttendanceStatus.Cheating, "Cheating"],
  [MarkAttendanceStatus.Blocked, "Blocked"],
];

function createDraft(entry?: {
  marksObtained: number;
  graceMarks: number;
  attendanceStatus: MarkAttendanceStatusValue;
  remarks?: string | null;
}): Draft {
  return {
    marksObtained: entry ? String(entry.marksObtained) : "",
    graceMarks: String(entry?.graceMarks ?? 0),
    attendanceStatus: entry?.attendanceStatus ?? MarkAttendanceStatus.Present,
    remarks: entry?.remarks ?? "",
  };
}

export function MarksEntryPage() {
  const { hasPermission } = usePermissions();
  const examsQuery = useExams();
  const teachersQuery = useTeachers();
  const studentsQuery = useStudents();
  const assignmentsQuery = useSubjectTeachers();
  const employeesQuery = useEmployees(hasPermission(Permission.EmployeeView));
  const [examId, setExamId] = useState("");
  const [scheduleId, setScheduleId] = useState("");
  const [teacherId, setTeacherId] = useState("");
  const [drafts, setDrafts] = useState<Record<number, Draft>>({});
  const selectedExamId = examId ? Number(examId) : null;
  const selectedScheduleId = scheduleId ? Number(scheduleId) : null;
  const examQuery = useExamDetails(selectedExamId);
  const entriesQuery = useMarkEntries(selectedScheduleId);
  const saveEntries = useSaveBulkMarkEntries();
  const submitEntries = useSubmitMarkEntries();
  const lockActions = useMarkEntryLock();
  const selectedExam = examQuery.data;
  const selectedSchedule = selectedExam?.schedules.find(
    (item) => item.id === selectedScheduleId
  );
  const entries = entriesQuery.data ?? EMPTY_LIST;
  const exams = examsQuery.data ?? EMPTY_LIST;
  const teachers = teachersQuery.data ?? EMPTY_LIST;
  const students = studentsQuery.data ?? EMPTY_LIST;
  const assignments = assignmentsQuery.data ?? EMPTY_LIST;
  const employees = employeesQuery.data ?? EMPTY_LIST;
  const publishedExams = useMemo(
    () => exams.filter((exam) => exam.status === ExamStatus.Published),
    [exams]
  );
  const classStudents = useMemo(
    () =>
      students
        .filter((student) => student.classId === selectedSchedule?.classId)
        .sort((left, right) => {
          const rollCompare = left.rollNo.localeCompare(right.rollNo, undefined, {
            numeric: true,
          });
          return rollCompare || left.fullName.localeCompare(right.fullName);
        }),
    [students, selectedSchedule?.classId]
  );
  const assignedTeacherIds = useMemo(
    () =>
      new Set(
        assignments
          .filter((assignment) => assignment.subjectId === selectedSchedule?.subjectId)
          .map((assignment) => assignment.teacherId)
      ),
    [assignments, selectedSchedule?.subjectId]
  );
  const assignedTeachers = teachers.filter((teacher) =>
    assignedTeacherIds.has(teacher.id)
  );
  const employeeById = useMemo(
    () => new Map(employees.map((employee) => [employee.id, employee])),
    [employees]
  );
  const entriesByStudentId = useMemo(
    () => new Map(entries.map((entry) => [entry.studentId, entry])),
    [entries]
  );
  const hasUnsavedChanges = classStudents.some((student) => {
    const entry = entriesByStudentId.get(student.id);
    const draft = drafts[student.id];
    if (!entry) return true;
    if (!draft) return false;

    return (
      draft.marksObtained.trim() === "" ||
      Number(draft.marksObtained) !== entry.marksObtained ||
      Number(draft.graceMarks || 0) !== entry.graceMarks ||
      draft.attendanceStatus !== entry.attendanceStatus ||
      (draft.remarks.trim() || "") !== (entry.remarks ?? "")
    );
  });
  const canSave = hasPermission(Permission.MarksEntryCreate);
  const canSubmit = hasPermission(Permission.MarksEntryEdit);
  const canLock = hasPermission(Permission.MarksEntryPublish);
  const isLocked = entries.some((entry) => entry.isLocked);
  const submittedCount = entries.filter(
    (entry) => entry.entryStatus === MarkEntryStatus.Submitted
  ).length;

  useEffect(() => {
    const next: Record<number, Draft> = {};
    entries.forEach((entry) => {
      next[entry.studentId] = createDraft(entry);
    });
    // The server remains the source of truth after loading or saving a paper.
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setDrafts(next);
  }, [entries, selectedScheduleId]);

  function selectExam(value: string) {
    setExamId(value);
    setScheduleId("");
    setTeacherId("");
    setDrafts({});
  }

  function selectSchedule(value: string) {
    setScheduleId(value);
    setTeacherId("");
    setDrafts({});
  }

  function updateDraft(studentId: number, patch: Partial<Draft>) {
    setDrafts((current) => ({
      ...current,
      [studentId]: {
        ...(current[studentId] ?? createDraft()),
        ...patch,
      },
    }));
  }

  function updateAttendance(studentId: number, status: MarkAttendanceStatusValue) {
    updateDraft(studentId, {
      attendanceStatus: status,
      ...(status === MarkAttendanceStatus.Present
        ? {}
        : { marksObtained: "0", graceMarks: "0" }),
    });
  }

  function validateDrafts() {
    if (!selectedSchedule || !teacherId || !assignedTeacherIds.has(Number(teacherId))) {
      toast.error("Select a teacher assigned to this subject.");
      return false;
    }

    const invalidStudent = classStudents.find((student) => {
      const draft = drafts[student.id];
      if (!draft) return true;
      if (draft.attendanceStatus !== MarkAttendanceStatus.Present) return false;

      const marksText = draft.marksObtained.trim();
      const marks = Number(marksText);
      const grace = Number(draft.graceMarks || 0);

      return (
        marksText.length === 0 ||
        !Number.isFinite(marks) ||
        marks < 0 ||
        !Number.isFinite(grace) ||
        grace < 0 ||
        marks + grace > selectedSchedule.fullMarks
      );
    });

    if (invalidStudent) {
      toast.error(
        `Check marks and grace marks for ${invalidStudent.fullName}. Their total cannot exceed ${selectedSchedule.fullMarks}.`
      );
      return false;
    }
    return classStudents.length > 0;
  }

  async function handleSave() {
    if (!selectedScheduleId || !validateDrafts()) return;
    try {
      await saveEntries.mutateAsync({
        examScheduleId: selectedScheduleId,
        teacherId: Number(teacherId),
        entries: classStudents.map((student) => ({
          studentId: student.id,
          marksObtained: Number(drafts[student.id].marksObtained),
          graceMarks: Number(drafts[student.id].graceMarks || 0),
          attendanceStatus: drafts[student.id].attendanceStatus,
          remarks: drafts[student.id].remarks.trim() || null,
        })),
      });
      toast.success("Marks saved as draft.");
    } catch (error) {
      toast.error(getResultErrorMessage(error));
    }
  }

  async function handleSubmit() {
    if (!selectedScheduleId || !teacherId || entries.length === 0) return;
    try {
      await submitEntries.mutateAsync({
        examScheduleId: selectedScheduleId,
        teacherId: Number(teacherId),
      });
      toast.success("Marks submitted.");
    } catch (error) {
      toast.error(getResultErrorMessage(error));
    }
  }

  async function handleLock(shouldLock: boolean) {
    if (!selectedScheduleId || entries.length === 0) return;
    try {
      await (shouldLock ? lockActions.lock : lockActions.unlock).mutateAsync(
        selectedScheduleId
      );
      toast.success(shouldLock ? "Marks locked." : "Marks unlocked.");
    } catch (error) {
      toast.error(getResultErrorMessage(error));
    }
  }

  if (!hasPermission(Permission.MarksEntryView)) {
    return (
      <Card>
        <CardContent className="p-6 text-sm text-destructive">
          You do not have permission to view marks.
        </CardContent>
      </Card>
    );
  }

  const loadingSelectionData =
    examsQuery.isPending ||
    teachersQuery.isPending ||
    studentsQuery.isPending ||
    assignmentsQuery.isPending;
  const selectionDataError =
    examsQuery.isError ||
    teachersQuery.isError ||
    studentsQuery.isError ||
    assignmentsQuery.isError;

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Marks entry</h1>
          <p className="text-sm text-muted-foreground">
            Record each student&apos;s marks and attendance, save a draft, then
            submit the paper.
          </p>
        </div>
        <Link
          to="/exams"
          className="inline-flex items-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent"
        >
          Exams
        </Link>
      </div>

      {loadingSelectionData ? (
        <div className="flex items-center gap-2 py-4 text-sm text-muted-foreground">
          <Loader2 aria-hidden="true" className="h-4 w-4 animate-spin" />
          Loading exams, students, and teacher assignments…
        </div>
      ) : selectionDataError ? (
        <Card className="border-destructive/30">
          <CardContent className="flex flex-col gap-3 p-5 sm:flex-row sm:items-center sm:justify-between">
            <p className="text-sm text-destructive">
              Unable to load the data needed for mark entry. Check your access
              to exams, students, teachers, and subject-teacher assignments.
            </p>
            <Button
              variant="outline"
              onClick={() => {
                void examsQuery.refetch();
                void teachersQuery.refetch();
                void studentsQuery.refetch();
                void assignmentsQuery.refetch();
              }}
            >
              Try again
            </Button>
          </CardContent>
        </Card>
      ) : (
        <Card>
          <CardHeader>
            <CardTitle>Paper selection</CardTitle>
            <CardDescription>
              Marks can only be entered for published exams. The teacher must
              be assigned to the selected subject.
            </CardDescription>
          </CardHeader>
          <CardContent className="grid gap-4 md:grid-cols-3">
            <div className="space-y-1.5">
              <label htmlFor="marks-exam" className="text-sm font-medium">
                Exam
              </label>
              <select
                id="marks-exam"
                value={examId}
                onChange={(event) => selectExam(event.target.value)}
                className="w-full rounded-md border bg-background px-3 py-2"
              >
                <option value="">Select published exam</option>
                {publishedExams.map((item) => (
                  <option key={item.id} value={item.id}>
                    {item.name}
                  </option>
                ))}
              </select>
              {publishedExams.length === 0 && (
                <p className="text-xs text-muted-foreground">
                  No published exams are available for mark entry.
                </p>
              )}
            </div>

            <div className="space-y-1.5">
              <label htmlFor="marks-paper" className="text-sm font-medium">
                Paper
              </label>
              <select
                id="marks-paper"
                value={scheduleId}
                onChange={(event) => selectSchedule(event.target.value)}
                disabled={!selectedExam}
                className="w-full rounded-md border bg-background px-3 py-2 disabled:opacity-60"
              >
                <option value="">Select paper</option>
                {selectedExam?.schedules.map((item) => (
                  <option key={item.id} value={item.id}>
                    {item.subjectName} · {item.className}
                  </option>
                ))}
              </select>
              {selectedExam && selectedExam.schedules.length === 0 && (
                <p className="text-xs text-muted-foreground">
                  This exam has no scheduled papers.
                </p>
              )}
            </div>

            <div className="space-y-1.5">
              <label htmlFor="marks-teacher" className="text-sm font-medium">
                Entered by
              </label>
              <select
                id="marks-teacher"
                value={teacherId}
                onChange={(event) => setTeacherId(event.target.value)}
                disabled={!selectedSchedule || assignedTeachers.length === 0}
                className="w-full rounded-md border bg-background px-3 py-2 disabled:opacity-60"
              >
                <option value="">Select assigned teacher</option>
                {assignedTeachers.map((teacher) => {
                  const employee = employeeById.get(teacher.employeeId);
                  return (
                    <option key={teacher.id} value={teacher.id}>
                      {employee?.fullName ?? `Teacher #${teacher.id}`}
                    </option>
                  );
                })}
              </select>
              {selectedSchedule && assignedTeachers.length === 0 && (
                <p className="text-xs text-destructive">
                  No teacher is assigned to this subject. Assign a teacher
                  before entering marks.
                </p>
              )}
              {employeesQuery.isError && (
                <p className="text-xs text-muted-foreground">
                  Staff names are unavailable; teacher IDs are shown instead.
                </p>
              )}
            </div>
          </CardContent>
        </Card>
      )}

      {selectedExamId && examQuery.isPending && (
        <p className="text-sm text-muted-foreground">
          <Loader2 aria-hidden="true" className="mr-2 inline h-4 w-4 animate-spin" />
          Loading scheduled papers…
        </p>
      )}
      {selectedExamId && examQuery.isError && (
        <Card className="border-destructive/30">
          <CardContent className="flex flex-col gap-3 p-5 sm:flex-row sm:items-center sm:justify-between">
            <p className="text-sm text-destructive">
              Unable to load the selected exam&apos;s scheduled papers.
            </p>
            <Button
              variant="outline"
              onClick={() => void examQuery.refetch()}
            >
              Try again
            </Button>
          </CardContent>
        </Card>
      )}

      {selectedSchedule && (
        <Card>
          <CardHeader>
            <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
              <div>
                <CardTitle>
                  {selectedSchedule.subjectName} · {selectedSchedule.className}
                </CardTitle>
                <CardDescription>
                  Full marks {selectedSchedule.fullMarks} · Pass marks{" "}
                  {selectedSchedule.passMarks} · {entries.length}/
                  {classStudents.length} entries saved
                  {entries.length > 0 &&
                    ` · ${submittedCount} submitted · ${isLocked ? "Locked" : "Unlocked"}`}
                </CardDescription>
              </div>
              <div className="flex flex-col gap-2">
                <div className="flex flex-wrap gap-2 md:justify-end">
                  {canSave && (
                    <Button
                      disabled={
                        isLocked ||
                        entriesQuery.isPending ||
                        classStudents.length === 0 ||
                        !teacherId ||
                        saveEntries.isPending
                      }
                      onClick={() => void handleSave()}
                    >
                      {saveEntries.isPending ? (
                        <Loader2
                          aria-hidden="true"
                          className="mr-2 h-4 w-4 animate-spin"
                        />
                      ) : (
                        <Save aria-hidden="true" className="mr-2 h-4 w-4" />
                      )}
                      Save draft
                    </Button>
                  )}
                  {canSubmit && (
                    <Button
                      variant="outline"
                      disabled={
                        entriesQuery.isPending ||
                        entries.length === 0 ||
                        isLocked ||
                        hasUnsavedChanges ||
                        !teacherId ||
                        submitEntries.isPending
                      }
                      onClick={() => void handleSubmit()}
                    >
                      {submitEntries.isPending ? (
                        <Loader2
                          aria-hidden="true"
                          className="mr-2 h-4 w-4 animate-spin"
                        />
                      ) : (
                        <Send aria-hidden="true" className="mr-2 h-4 w-4" />
                      )}
                      Submit
                    </Button>
                  )}
                  {canLock && (
                    <Button
                      variant="outline"
                      disabled={
                        entriesQuery.isPending ||
                        entries.length === 0 ||
                        hasUnsavedChanges ||
                        lockActions.lock.isPending ||
                        lockActions.unlock.isPending
                      }
                      onClick={() => void handleLock(!isLocked)}
                    >
                      {isLocked ? (
                        <Unlock
                          aria-hidden="true"
                          className="mr-2 h-4 w-4"
                        />
                      ) : (
                        <Lock aria-hidden="true" className="mr-2 h-4 w-4" />
                      )}
                      {isLocked ? "Unlock" : "Lock"}
                    </Button>
                  )}
                </div>
                {hasUnsavedChanges && entries.length > 0 && (
                  <p className="text-xs text-muted-foreground md:text-right">
                    Save your latest changes before submitting or locking this
                    paper.
                  </p>
                )}
              </div>
            </div>
          </CardHeader>
          <CardContent className="space-y-3">
            {entriesQuery.isPending ? (
              <p className="text-sm text-muted-foreground">
                <Loader2
                  aria-hidden="true"
                  className="mr-2 inline h-4 w-4 animate-spin"
                />
                Loading saved marks…
              </p>
            ) : entriesQuery.isError ? (
              <div className="flex flex-col gap-3 rounded-md border border-destructive/30 p-4 sm:flex-row sm:items-center sm:justify-between">
                <p className="text-sm text-destructive">
                  Unable to load marks for this paper.
                </p>
                <Button
                  variant="outline"
                  onClick={() => void entriesQuery.refetch()}
                >
                  Try again
                </Button>
              </div>
            ) : classStudents.length === 0 ? (
              <p className="rounded-md border border-dashed p-6 text-center text-sm text-muted-foreground">
                No students are assigned to this class.
              </p>
            ) : (
              <>
                <div
                  aria-hidden="true"
                  className="hidden grid-cols-[minmax(150px,1fr)_110px_110px_170px_minmax(180px,1.2fr)] gap-3 border-b px-2 pb-2 text-xs font-medium uppercase tracking-wide text-muted-foreground md:grid"
                >
                  <span>Student</span>
                  <span>Marks</span>
                  <span>Grace</span>
                  <span>Attendance</span>
                  <span>Remarks</span>
                </div>
                {classStudents.map((student) => {
                  const draft = drafts[student.id] ?? createDraft();
                  const editable = canSave && !isLocked;
                  const notPresent =
                    draft.attendanceStatus !== MarkAttendanceStatus.Present;
                  return (
                    <div
                      key={student.id}
                      className="grid gap-3 rounded-lg border p-3 md:grid-cols-[minmax(150px,1fr)_110px_110px_170px_minmax(180px,1.2fr)] md:items-center md:gap-3 md:rounded-none md:border-0 md:border-b md:px-2 md:py-3"
                    >
                      <div className="min-w-0">
                        <p className="truncate text-sm font-medium">
                          {student.fullName}
                        </p>
                        <p className="text-xs text-muted-foreground">
                          Roll {student.rollNo}
                        </p>
                      </div>
                      <label className="space-y-1 text-xs text-muted-foreground md:text-foreground">
                        <span className="md:hidden">Marks</span>
                        <input
                          type="number"
                          min="0"
                          max={selectedSchedule.fullMarks}
                          step="0.01"
                          value={draft.marksObtained}
                          required={!notPresent}
                          disabled={!editable || notPresent}
                          aria-label={`Marks for ${student.fullName}`}
                          onChange={(event) =>
                            updateDraft(student.id, {
                              marksObtained: event.target.value,
                            })
                          }
                          className="w-full rounded-md border bg-background px-2 py-1.5 text-sm disabled:bg-muted"
                        />
                      </label>
                      <label className="space-y-1 text-xs text-muted-foreground md:text-foreground">
                        <span className="md:hidden">Grace marks</span>
                        <input
                          type="number"
                          min="0"
                          max={selectedSchedule.fullMarks}
                          step="0.01"
                          value={draft.graceMarks}
                          disabled={!editable || notPresent}
                          aria-label={`Grace marks for ${student.fullName}`}
                          onChange={(event) =>
                            updateDraft(student.id, {
                              graceMarks: event.target.value,
                            })
                          }
                          className="w-full rounded-md border bg-background px-2 py-1.5 text-sm disabled:bg-muted"
                        />
                      </label>
                      <label className="space-y-1 text-xs text-muted-foreground md:text-foreground">
                        <span className="md:hidden">Attendance</span>
                        <select
                          value={draft.attendanceStatus}
                          disabled={!editable}
                          aria-label={`Attendance status for ${student.fullName}`}
                          onChange={(event) =>
                            updateAttendance(
                              student.id,
                              Number(
                                event.target.value
                              ) as MarkAttendanceStatusValue
                            )
                          }
                          className="w-full rounded-md border bg-background px-2 py-1.5 text-sm disabled:bg-muted"
                        >
                          {attendanceOptions.map(([value, label]) => (
                            <option key={value} value={value}>
                              {label}
                            </option>
                          ))}
                        </select>
                      </label>
                      <label className="space-y-1 text-xs text-muted-foreground md:text-foreground">
                        <span className="md:hidden">Remarks</span>
                        <input
                          value={draft.remarks}
                          maxLength={500}
                          disabled={!editable}
                          aria-label={`Remarks for ${student.fullName}`}
                          onChange={(event) =>
                            updateDraft(student.id, {
                              remarks: event.target.value,
                            })
                          }
                          className="w-full rounded-md border bg-background px-2 py-1.5 text-sm disabled:bg-muted"
                        />
                      </label>
                    </div>
                  );
                })}
              </>
            )}
          </CardContent>
        </Card>
      )}
    </div>
  );
}
