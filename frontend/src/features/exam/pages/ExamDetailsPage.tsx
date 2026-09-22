import { useState } from "react";
import { Link, useParams } from "react-router-dom";
import { Loader2, Pencil, Plus, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useSchoolClasses, useSubjects } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { getExamErrorMessage } from "../api/exam.api";
import {
  useCreateExamSchedule,
  useDeleteExamSchedule,
  useExamDetails,
  useExamLifecycle,
  useUpdateExam,
  useUpdateExamSchedule,
} from "../hooks/useExamData";
import { examStatusLabel, ExamStatus, type ExamScheduleDto } from "../types/exam.types";

function toDateTimeLocal(value?: string | null) {
  if (!value) return "";
  return value.slice(0, 16);
}

export function ExamDetailsPage() {
  const { id } = useParams();
  const examId = Number(id);
  const { hasPermission } = usePermissions();
  const { data: exam, isPending, isError } = useExamDetails(Number.isNaN(examId) ? null : examId);
  const { data: classes = [] } = useSchoolClasses();
  const { data: subjects = [] } = useSubjects();
  const updateExam = useUpdateExam();
  const createSchedule = useCreateExamSchedule();
  const updateSchedule = useUpdateExamSchedule();
  const deleteSchedule = useDeleteExamSchedule();
  const lifecycle = useExamLifecycle();

  const [examName, setExamName] = useState("");
  const [scheduleDraft, setScheduleDraft] = useState({
    classId: "",
    subjectId: "",
    examDate: "",
    fullMarks: "100",
    passMarks: "33",
  });
  const [editingSchedule, setEditingSchedule] = useState<ExamScheduleDto | null>(null);

  const lockedCore = exam ? exam.status !== ExamStatus.Draft : true;
  const scheduleLocked = exam
    ? exam.status === ExamStatus.Completed || exam.status === ExamStatus.Cancelled
    : true;

  if (isPending) {
    return (
      <div className="py-12 text-center text-sm text-muted-foreground">
        <Loader2 className="mr-2 inline h-4 w-4 animate-spin" /> Loading exam…
      </div>
    );
  }

  if (isError || !exam) {
    return (
      <div className="mx-auto max-w-4xl rounded-lg border p-6 text-sm text-destructive">
        Unable to load this exam. <Link to="/exams/list" className="underline">Back to list</Link>
      </div>
    );
  }

  async function handleUpdateExam(event: React.FormEvent) {
    event.preventDefault();
    if (!exam) return;
    try {
      await updateExam.mutateAsync({
        id: exam.id,
        payload: {
          id: exam.id,
          name: examName.trim() || exam.name,
          examTypeId: exam.examTypeId,
          academicYearId: exam.academicYearId,
        },
      });
      toast.success("Exam updated.");
    } catch (error) {
      toast.error(getExamErrorMessage(error));
    }
  }

  async function handleSaveSchedule(event: React.FormEvent) {
    event.preventDefault();
    if (!exam) return;
    const fullMarks = Number(scheduleDraft.fullMarks);
    const passMarks = Number(scheduleDraft.passMarks);
    if (!scheduleDraft.classId || !scheduleDraft.subjectId || !scheduleDraft.examDate) {
      toast.error("Class, subject, and exam date are required.");
      return;
    }
    if (!Number.isFinite(fullMarks) || !Number.isFinite(passMarks) || fullMarks <= 0 || passMarks < 0 || passMarks > fullMarks) {
      toast.error("Pass marks must be between 0 and full marks.");
      return;
    }
    const payload = {
      examId: exam.id,
      classId: Number(scheduleDraft.classId),
      subjectId: Number(scheduleDraft.subjectId),
      examDate: scheduleDraft.examDate,
      fullMarks,
      passMarks,
    };
    try {
      if (editingSchedule) {
        await updateSchedule.mutateAsync({
          id: editingSchedule.id,
          payload: { ...payload, id: editingSchedule.id },
        });
        toast.success("Schedule updated.");
      } else {
        await createSchedule.mutateAsync(payload);
        toast.success("Schedule added.");
      }
      setEditingSchedule(null);
      setScheduleDraft({ classId: "", subjectId: "", examDate: "", fullMarks: "100", passMarks: "33" });
    } catch (error) {
      toast.error(getExamErrorMessage(error));
    }
  }

  const nameValue = examName || exam.name;

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">{exam.name}</h1>
          <p className="text-sm text-muted-foreground">
            {exam.examTypeName} • {exam.academicYearName} • {examStatusLabel(exam.status)}
          </p>
        </div>
        <div className="flex flex-wrap gap-2">
          <Link to="/exams/list" className="inline-flex items-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent">Back</Link>
          {exam.status === ExamStatus.Draft && hasPermission(Permission.ExamPublish) ? (
            <Button onClick={() => void lifecycle.publish.mutateAsync(exam.id).then(() => toast.success("Published.")).catch((error) => toast.error(getExamErrorMessage(error)))}>Publish</Button>
          ) : null}
          {exam.status === ExamStatus.Published && hasPermission(Permission.ExamComplete) ? (
            <Button onClick={() => void lifecycle.complete.mutateAsync(exam.id).then(() => toast.success("Completed.")).catch((error) => toast.error(getExamErrorMessage(error)))}>Complete</Button>
          ) : null}
          {(exam.status === ExamStatus.Draft || exam.status === ExamStatus.Published) && hasPermission(Permission.ExamCancel) ? (
            <Button variant="outline" onClick={() => void lifecycle.cancel.mutateAsync(exam.id).then(() => toast.success("Cancelled.")).catch((error) => toast.error(getExamErrorMessage(error)))}>Cancel</Button>
          ) : null}
          {exam.status === ExamStatus.Cancelled && hasPermission(Permission.ExamPublish) ? (
            <Button variant="outline" onClick={() => void lifecycle.reopen.mutateAsync(exam.id).then(() => toast.success("Reopened.")).catch((error) => toast.error(getExamErrorMessage(error)))}>Reopen</Button>
          ) : null}
        </div>
      </div>

      {!lockedCore && hasPermission(Permission.ExamEdit) ? (
        <Card>
          <CardHeader>
            <CardTitle>Edit exam</CardTitle>
            <CardDescription>Core details can change only while the exam is Draft.</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={(event) => void handleUpdateExam(event)} className="flex flex-col gap-3 sm:flex-row">
              <input value={nameValue} onChange={(event) => setExamName(event.target.value)} className="flex-1 rounded-md border bg-background px-3 py-2" />
              <Button type="submit" disabled={updateExam.isPending}>Save name</Button>
            </form>
          </CardContent>
        </Card>
      ) : null}

      <Card>
        <CardHeader>
          <CardTitle>Schedules</CardTitle>
          <CardDescription>
            {exam.totalSchedules} paper{exam.totalSchedules === 1 ? "" : "s"}. Schedule changes are blocked after Complete or Cancel.
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          {!scheduleLocked && hasPermission(Permission.ExamScheduleCreate) ? (
            <form onSubmit={(event) => void handleSaveSchedule(event)} className="grid gap-3 md:grid-cols-6">
              <select required value={scheduleDraft.classId} onChange={(event) => setScheduleDraft((value) => ({ ...value, classId: event.target.value }))} className="rounded-md border bg-background px-3 py-2">
                <option value="">Class</option>
                {classes.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
              </select>
              <select required value={scheduleDraft.subjectId} onChange={(event) => setScheduleDraft((value) => ({ ...value, subjectId: event.target.value }))} className="rounded-md border bg-background px-3 py-2">
                <option value="">Subject</option>
                {subjects.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
              </select>
              <input required type="datetime-local" value={scheduleDraft.examDate} onChange={(event) => setScheduleDraft((value) => ({ ...value, examDate: event.target.value }))} className="rounded-md border bg-background px-3 py-2 md:col-span-2" />
              <input required type="number" min="1" placeholder="Full" value={scheduleDraft.fullMarks} onChange={(event) => setScheduleDraft((value) => ({ ...value, fullMarks: event.target.value }))} className="rounded-md border bg-background px-3 py-2" />
              <input required type="number" min="1" placeholder="Pass" value={scheduleDraft.passMarks} onChange={(event) => setScheduleDraft((value) => ({ ...value, passMarks: event.target.value }))} className="rounded-md border bg-background px-3 py-2" />
              <div className="flex gap-2 md:col-span-6">
                <Button type="submit" disabled={createSchedule.isPending || updateSchedule.isPending}>
                  {createSchedule.isPending || updateSchedule.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                  {editingSchedule ? "Save paper" : "Add paper"}
                </Button>
                {editingSchedule ? (
                  <Button type="button" variant="outline" onClick={() => { setEditingSchedule(null); setScheduleDraft({ classId: "", subjectId: "", examDate: "", fullMarks: "100", passMarks: "33" }); }}>Cancel edit</Button>
                ) : null}
              </div>
            </form>
          ) : null}

          {(exam.schedules ?? []).length === 0 ? (
            <p className="text-sm text-muted-foreground">No papers scheduled yet.</p>
          ) : (
            <div className="space-y-2">
              {(exam.schedules ?? []).map((item) => (
                <div key={item.id} className="flex flex-col gap-2 rounded-lg border p-3 md:flex-row md:items-center md:justify-between">
                  <div>
                    <p className="font-medium">{item.subjectName} • {item.className}</p>
                    <p className="text-sm text-muted-foreground">
                      {new Date(item.examDate).toLocaleString("en-BD")} · Full {item.fullMarks} · Pass {item.passMarks}
                    </p>
                  </div>
                  {!scheduleLocked ? (
                    <div className="flex gap-2">
                      {hasPermission(Permission.ExamScheduleEdit) ? (
                        <Button type="button" variant="outline" onClick={() => {
                          setEditingSchedule(item);
                          setScheduleDraft({
                            classId: String(item.classId),
                            subjectId: String(item.subjectId),
                            examDate: toDateTimeLocal(item.examDate),
                            fullMarks: String(item.fullMarks),
                            passMarks: String(item.passMarks),
                          });
                        }}><Pencil className="h-4 w-4" /></Button>
                      ) : null}
                      {hasPermission(Permission.ExamScheduleDelete) ? (
                        <Button type="button" variant="outline" onClick={() => {
                          if (!window.confirm("Delete this paper?")) return;
                          void deleteSchedule.mutateAsync(item.id).then(() => toast.success("Deleted.")).catch((error) => toast.error(getExamErrorMessage(error)));
                        }}><Trash2 className="h-4 w-4" /></Button>
                      ) : null}
                    </div>
                  ) : null}
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
