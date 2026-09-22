import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, Save, Send, Unlock, Lock } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useExams, useExamDetails } from "@/features/exam/hooks/useExamData";
import { useStudents, useTeachers } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { getResultErrorMessage } from "../api/result.api";
import {
  useMarkEntries,
  useMarkEntryLock,
  useSaveBulkMarkEntries,
  useSubmitMarkEntries,
} from "../hooks/useResultData";
import { MarkAttendanceStatus, type MarkAttendanceStatusValue } from "../types/result.types";

type Draft = {
  marksObtained: string;
  graceMarks: string;
  attendanceStatus: MarkAttendanceStatusValue;
  remarks: string;
};

const attendanceOptions: Array<[MarkAttendanceStatusValue, string]> = [
  [MarkAttendanceStatus.Present, "Present"],
  [MarkAttendanceStatus.Absent, "Absent"],
  [MarkAttendanceStatus.Medical, "Medical"],
  [MarkAttendanceStatus.Withheld, "Withheld"],
  [MarkAttendanceStatus.Incomplete, "Incomplete"],
  [MarkAttendanceStatus.Excused, "Excused"],
  [MarkAttendanceStatus.Late, "Late"],
];

export function MarksEntryPage() {
  const { hasPermission } = usePermissions();
  const { data: exams = [] } = useExams();
  const { data: teachers = [] } = useTeachers();
  const { data: students = [] } = useStudents();
  const [examId, setExamId] = useState("");
  const [scheduleId, setScheduleId] = useState("");
  const [teacherId, setTeacherId] = useState("");
  const [drafts, setDrafts] = useState<Record<number, Draft>>({});
  const selectedExamId = examId ? Number(examId) : null;
  const selectedScheduleId = scheduleId ? Number(scheduleId) : null;
  const { data: exam } = useExamDetails(selectedExamId);
  const { data: entries = [], isPending, isError } = useMarkEntries(selectedScheduleId);
  const saveEntries = useSaveBulkMarkEntries();
  const submitEntries = useSubmitMarkEntries();
  const lockActions = useMarkEntryLock();
  const selectedSchedule = exam?.schedules.find((item) => item.id === selectedScheduleId);
  const classStudents = useMemo(
    () => students.filter((student) => student.classId === selectedSchedule?.classId),
    [students, selectedSchedule?.classId]
  );
  const canEdit = hasPermission(Permission.MarksEntryCreate) || hasPermission(Permission.MarksEntryEdit);
  const isLocked = entries.some((entry) => entry.isLocked);

  useEffect(() => {
    const next: Record<number, Draft> = {};
    entries.forEach((entry) => {
      next[entry.studentId] = {
        marksObtained: String(entry.marksObtained ?? ""),
        graceMarks: String(entry.graceMarks ?? 0),
        attendanceStatus: entry.attendanceStatus,
        remarks: entry.remarks ?? "",
      };
    });
    // The query response is the source of truth when a paper is selected or refreshed.
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setDrafts(next);
  }, [entries, selectedScheduleId]);

  function selectExam(value: string) {
    setExamId(value);
    setScheduleId("");
    setDrafts({});
  }

  function updateDraft(studentId: number, patch: Partial<Draft>) {
    setDrafts((current) => ({
      ...current,
      [studentId]: {
        ...(current[studentId] ?? {
          marksObtained: "",
          graceMarks: "0",
          attendanceStatus: MarkAttendanceStatus.Present,
          remarks: "",
        }),
        ...patch,
      },
    }));
  }

  async function handleSave() {
    if (!selectedScheduleId || !teacherId || !selectedSchedule) return;
    const invalid = classStudents.some((student) => {
      const draft = drafts[student.id];
      const marks = Number(draft?.marksObtained);
      const grace = Number(draft?.graceMarks || 0);
      return !draft || !Number.isFinite(marks) || marks < 0 || marks > selectedSchedule.fullMarks || !Number.isFinite(grace) || grace < 0;
    });
    if (invalid) {
      toast.error("Enter valid marks for every student before saving.");
      return;
    }
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
    if (!selectedScheduleId || !teacherId) return;
    try {
      await submitEntries.mutateAsync({ examScheduleId: selectedScheduleId, teacherId: Number(teacherId) });
      toast.success("Marks submitted.");
    } catch (error) {
      toast.error(getResultErrorMessage(error));
    }
  }

  async function handleLock(shouldLock: boolean) {
    if (!selectedScheduleId) return;
    try {
      await (shouldLock ? lockActions.lock : lockActions.unlock).mutateAsync(selectedScheduleId);
      toast.success(shouldLock ? "Marks locked." : "Marks unlocked.");
    } catch (error) {
      toast.error(getResultErrorMessage(error));
    }
  }

  if (!hasPermission(Permission.MarksEntryView)) {
    return <Card><CardContent className="p-6 text-sm text-destructive">You do not have permission to view marks.</CardContent></Card>;
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Marks entry</h1>
          <p className="text-sm text-muted-foreground">Enter marks by exam paper, save drafts, then submit for result calculation.</p>
        </div>
        <Link to="/exams" className="inline-flex items-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent">Exams</Link>
      </div>

      <Card>
        <CardHeader><CardTitle>Paper selection</CardTitle><CardDescription>Choose the exam, paper, and teacher responsible for this entry.</CardDescription></CardHeader>
        <CardContent className="grid gap-3 md:grid-cols-3">
          <select value={examId} onChange={(event) => selectExam(event.target.value)} className="rounded-md border bg-background px-3 py-2">
            <option value="">Select exam</option>
            {exams.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
          </select>
          <select value={scheduleId} onChange={(event) => { setScheduleId(event.target.value); setDrafts({}); }} disabled={!exam} className="rounded-md border bg-background px-3 py-2">
            <option value="">Select paper</option>
            {exam?.schedules.map((item) => <option key={item.id} value={item.id}>{item.subjectName} · {item.className}</option>)}
          </select>
          <select value={teacherId} onChange={(event) => setTeacherId(event.target.value)} className="rounded-md border bg-background px-3 py-2">
            <option value="">Entered by teacher</option>
            {teachers.map((item) => <option key={item.id} value={item.id}>Teacher #{item.id}</option>)}
          </select>
        </CardContent>
      </Card>

      {selectedSchedule ? (
        <Card>
          <CardHeader>
            <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
              <div><CardTitle>{selectedSchedule.subjectName} · {selectedSchedule.className}</CardTitle><CardDescription>Full marks {selectedSchedule.fullMarks} · Pass marks {selectedSchedule.passMarks} {isLocked ? "· Locked" : "· Draft"}</CardDescription></div>
              <div className="flex flex-wrap gap-2">
                {hasPermission(Permission.MarksEntryCreate) || hasPermission(Permission.MarksEntryEdit) ? <Button disabled={!canEdit || isLocked || saveEntries.isPending} onClick={() => void handleSave()}><Save className="mr-2 h-4 w-4" />Save draft</Button> : null}
                {hasPermission(Permission.MarksEntryEdit) ? <Button variant="outline" disabled={isLocked || submitEntries.isPending} onClick={() => void handleSubmit()}><Send className="mr-2 h-4 w-4" />Submit</Button> : null}
                {hasPermission(Permission.MarksEntryPublish) ? <Button variant="outline" disabled={lockActions.lock.isPending || lockActions.unlock.isPending} onClick={() => void handleLock(isLocked ? false : true)}>{isLocked ? <Unlock className="mr-2 h-4 w-4" /> : <Lock className="mr-2 h-4 w-4" />}{isLocked ? "Unlock" : "Lock"}</Button> : null}
              </div>
            </div>
          </CardHeader>
          <CardContent>
            {isPending ? <p className="text-sm text-muted-foreground"><Loader2 className="mr-2 inline h-4 w-4 animate-spin" />Loading students…</p> : null}
            {isError ? <p className="text-sm text-destructive">Unable to load marks for this paper.</p> : null}
            {!isPending && classStudents.length === 0 ? <p className="text-sm text-muted-foreground">No students are assigned to this class.</p> : null}
            {classStudents.length > 0 ? (
              <div className="overflow-x-auto"><table className="w-full min-w-[850px] text-sm"><thead><tr className="border-b text-left"><th className="p-2">Student</th><th className="p-2">Marks</th><th className="p-2">Grace</th><th className="p-2">Attendance</th><th className="p-2">Remarks</th></tr></thead><tbody>
                {classStudents.map((student) => { const draft = drafts[student.id] ?? { marksObtained: "", graceMarks: "0", attendanceStatus: MarkAttendanceStatus.Present, remarks: "" }; return <tr key={student.id} className="border-b align-top"><td className="p-2"><p className="font-medium">{student.fullName}</p><p className="text-xs text-muted-foreground">Roll {student.rollNo}</p></td><td className="p-2"><input type="number" min="0" max={selectedSchedule.fullMarks} step="0.01" value={draft.marksObtained} disabled={isLocked} onChange={(event) => updateDraft(student.id, { marksObtained: event.target.value })} className="w-24 rounded-md border bg-background px-2 py-1.5" /></td><td className="p-2"><input type="number" min="0" step="0.01" value={draft.graceMarks} disabled={isLocked} onChange={(event) => updateDraft(student.id, { graceMarks: event.target.value })} className="w-20 rounded-md border bg-background px-2 py-1.5" /></td><td className="p-2"><select value={draft.attendanceStatus} disabled={isLocked} onChange={(event) => updateDraft(student.id, { attendanceStatus: Number(event.target.value) as MarkAttendanceStatusValue })} className="rounded-md border bg-background px-2 py-1.5">{attendanceOptions.map(([value, label]) => <option key={value} value={value}>{label}</option>)}</select></td><td className="p-2"><input value={draft.remarks} disabled={isLocked} maxLength={500} onChange={(event) => updateDraft(student.id, { remarks: event.target.value })} className="w-full rounded-md border bg-background px-2 py-1.5" /></td></tr>; })}
              </tbody></table></div>
            ) : null}
          </CardContent>
        </Card>
      ) : null}
    </div>
  );
}
