import { useMemo, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Loader2, Plus, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useAcademicYears } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { getExamErrorMessage } from "../api/exam.api";
import {
  useCreateExam,
  useDeleteExam,
  useExamLifecycle,
  useExams,
  useExamTypes,
} from "../hooks/useExamData";
import { examStatusLabel, ExamStatus } from "../types/exam.types";

export function ExamsPage() {
  const navigate = useNavigate();
  const { hasPermission } = usePermissions();
  const { data = [], isPending, isError } = useExams();
  const { data: types = [] } = useExamTypes();
  const { data: years = [] } = useAcademicYears();
  const createExam = useCreateExam();
  const deleteExam = useDeleteExam();
  const lifecycle = useExamLifecycle();
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [form, setForm] = useState({ name: "", examTypeId: "", academicYearId: "" });

  const canCreate = hasPermission(Permission.ExamCreate);
  const canDelete = hasPermission(Permission.ExamDelete);

  const filtered = useMemo(() => {
    const query = search.trim().toLowerCase();
    return data.filter((item) => {
      const matchesSearch =
        !query ||
        [item.name, item.examTypeName ?? "", item.academicYearName ?? ""].join(" ").toLowerCase().includes(query);
      const matchesStatus = statusFilter ? String(item.status) === statusFilter : true;
      return matchesSearch && matchesStatus;
    });
  }, [data, search, statusFilter]);

  async function handleCreate(event: React.FormEvent) {
    event.preventDefault();
    try {
      const created = await createExam.mutateAsync({
        name: form.name.trim(),
        examTypeId: Number(form.examTypeId),
        academicYearId: Number(form.academicYearId),
      });
      toast.success("Exam created as Draft.");
      setForm({ name: "", examTypeId: "", academicYearId: "" });
      navigate(`/exams/${created.id}`);
    } catch (error) {
      toast.error(getExamErrorMessage(error));
    }
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Exams</h1>
          <p className="text-sm text-muted-foreground">Only Draft exams can be edited. Add schedules from the exam details page.</p>
        </div>
        <Link to="/exams" className="inline-flex items-center justify-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent">
          Back
        </Link>
      </div>

      {canCreate ? (
        <Card>
          <CardHeader>
            <CardTitle>Create exam</CardTitle>
            <CardDescription>Name + type + year must be unique. New exams start as Draft.</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={(event) => void handleCreate(event)} className="grid gap-3 md:grid-cols-4">
              <input required maxLength={100} placeholder="Exam name" value={form.name} onChange={(event) => setForm((value) => ({ ...value, name: event.target.value }))} className="rounded-md border bg-background px-3 py-2" />
              <select required value={form.examTypeId} onChange={(event) => setForm((value) => ({ ...value, examTypeId: event.target.value }))} className="rounded-md border bg-background px-3 py-2">
                <option value="">Exam type</option>
                {types.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
              </select>
              <select required value={form.academicYearId} onChange={(event) => setForm((value) => ({ ...value, academicYearId: event.target.value }))} className="rounded-md border bg-background px-3 py-2">
                <option value="">Academic year</option>
                {years.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
              </select>
              <Button type="submit" disabled={createExam.isPending}>
                {createExam.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                Create
              </Button>
            </form>
          </CardContent>
        </Card>
      ) : null}

      <Card>
        <CardHeader>
          <CardTitle>All exams</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 md:grid-cols-2">
            <input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search exams" className="rounded-md border bg-background px-3 py-2" />
            <select value={statusFilter} onChange={(event) => setStatusFilter(event.target.value)} className="rounded-md border bg-background px-3 py-2">
              <option value="">All statuses</option>
              <option value={ExamStatus.Draft}>Draft</option>
              <option value={ExamStatus.Published}>Published</option>
              <option value={ExamStatus.Completed}>Completed</option>
              <option value={ExamStatus.Cancelled}>Cancelled</option>
            </select>
          </div>
          {isPending ? <p className="text-sm text-muted-foreground">Loading…</p> : null}
          {isError ? <p className="text-sm text-destructive">Unable to load exams.</p> : null}
          {filtered.map((exam) => (
            <div key={exam.id} className="flex flex-col gap-3 rounded-lg border p-4 md:flex-row md:items-center md:justify-between">
              <div>
                <p className="font-medium">{exam.name}</p>
                <p className="text-sm text-muted-foreground">
                  {exam.examTypeName} • {exam.academicYearName} • {examStatusLabel(exam.status)}
                </p>
              </div>
              <div className="flex flex-wrap gap-2">
                <Button type="button" variant="outline" onClick={() => navigate(`/exams/${exam.id}`)}>Open</Button>
                {exam.status === ExamStatus.Draft && hasPermission(Permission.ExamPublish) ? (
                  <Button type="button" variant="outline" onClick={() => void lifecycle.publish.mutateAsync(exam.id).then(() => toast.success("Published.")).catch((error) => toast.error(getExamErrorMessage(error)))}>Publish</Button>
                ) : null}
                {exam.status === ExamStatus.Published && hasPermission(Permission.ExamComplete) ? (
                  <Button type="button" variant="outline" onClick={() => void lifecycle.complete.mutateAsync(exam.id).then(() => toast.success("Completed.")).catch((error) => toast.error(getExamErrorMessage(error)))}>Complete</Button>
                ) : null}
                {(exam.status === ExamStatus.Draft || exam.status === ExamStatus.Published) && hasPermission(Permission.ExamCancel) ? (
                  <Button type="button" variant="outline" onClick={() => void lifecycle.cancel.mutateAsync(exam.id).then(() => toast.success("Cancelled.")).catch((error) => toast.error(getExamErrorMessage(error)))}>Cancel</Button>
                ) : null}
                {exam.status === ExamStatus.Cancelled && hasPermission(Permission.ExamPublish) ? (
                  <Button type="button" variant="outline" onClick={() => void lifecycle.reopen.mutateAsync(exam.id).then(() => toast.success("Reopened as Draft.")).catch((error) => toast.error(getExamErrorMessage(error)))}>Reopen</Button>
                ) : null}
                {canDelete && exam.status !== ExamStatus.Completed && exam.status !== ExamStatus.Cancelled ? (
                  <Button type="button" variant="outline" onClick={() => {
                    if (!window.confirm("Delete this exam?")) return;
                    void deleteExam.mutateAsync(exam.id).then(() => toast.success("Deleted.")).catch((error) => toast.error(getExamErrorMessage(error)));
                  }}><Trash2 className="h-4 w-4" /></Button>
                ) : null}
              </div>
            </div>
          ))}
        </CardContent>
      </Card>
    </div>
  );
}
