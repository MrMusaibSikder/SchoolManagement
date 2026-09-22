import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, Pencil, Plus, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useAcademicYears } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { getExamErrorMessage } from "../api/exam.api";
import {
  useCreateGradeSetup,
  useDeleteGradeSetup,
  useGradeSetups,
  useToggleGradeSetup,
  useUpdateGradeSetup,
} from "../hooks/useExamData";
import type { GradeSetupDto } from "../types/exam.types";

const emptyDraft = {
  academicYearId: "",
  gradeName: "",
  gradePoint: "5",
  minMarks: "80",
  maxMarks: "100",
  minPercentage: "80",
  maxPercentage: "100",
  isFail: false,
  displayOrder: "1",
};

export function GradeSetupPage() {
  const { hasPermission } = usePermissions();
  const canManage = hasPermission(Permission.GradeSetupManage);
  const { data: years = [] } = useAcademicYears();
  const { data = [], isPending, isError } = useGradeSetups();
  const createGrade = useCreateGradeSetup();
  const updateGrade = useUpdateGradeSetup();
  const deleteGrade = useDeleteGradeSetup();
  const toggleGrade = useToggleGradeSetup();
  const [yearFilter, setYearFilter] = useState("");
  const [editingId, setEditingId] = useState<number | null>(null);
  const [draft, setDraft] = useState(emptyDraft);

  const filtered = useMemo(
    () => (yearFilter ? data.filter((item) => String(item.academicYearId) === yearFilter) : data),
    [data, yearFilter]
  );

  function startEdit(item: GradeSetupDto) {
    setEditingId(item.id);
    setDraft({
      academicYearId: String(item.academicYearId),
      gradeName: item.gradeName,
      gradePoint: String(item.gradePoint),
      minMarks: String(item.minMarks),
      maxMarks: String(item.maxMarks),
      minPercentage: String(item.minPercentage),
      maxPercentage: String(item.maxPercentage),
      isFail: item.isFail,
      displayOrder: String(item.displayOrder),
    });
  }

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    const payload = {
      gradeName: draft.gradeName.trim(),
      gradePoint: Number(draft.gradePoint),
      minMarks: Number(draft.minMarks),
      maxMarks: Number(draft.maxMarks),
      minPercentage: Number(draft.minPercentage),
      maxPercentage: Number(draft.maxPercentage),
      isFail: draft.isFail,
      displayOrder: Number(draft.displayOrder),
    };
    try {
      if (editingId) {
        await updateGrade.mutateAsync({ id: editingId, payload: { ...payload, id: editingId } });
        toast.success("Grade band updated.");
      } else {
        await createGrade.mutateAsync({
          ...payload,
          academicYearId: Number(draft.academicYearId),
        });
        toast.success("Grade band created.");
      }
      setEditingId(null);
      setDraft(emptyDraft);
    } catch (error) {
      toast.error(getExamErrorMessage(error));
    }
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Grade setup</h1>
          <p className="text-sm text-muted-foreground">
            Percentage ranges cannot overlap within the same academic year.
          </p>
        </div>
        <Link to="/exams" className="inline-flex items-center justify-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent">
          Back
        </Link>
      </div>

      <div className="grid gap-6 lg:grid-cols-[0.95fr_1.05fr]">
        {canManage ? (
          <Card>
            <CardHeader>
              <CardTitle>{editingId ? "Edit grade" : "Create grade"}</CardTitle>
              <CardDescription>GPA scale is 0–5. Academic year is set only when creating.</CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={(event) => void handleSubmit(event)} className="grid gap-3 md:grid-cols-2">
                <select
                  required={!editingId}
                  disabled={Boolean(editingId)}
                  value={draft.academicYearId}
                  onChange={(event) => setDraft((value) => ({ ...value, academicYearId: event.target.value }))}
                  className="rounded-md border bg-background px-3 py-2 md:col-span-2"
                >
                  <option value="">Academic year</option>
                  {years.map((item) => (
                    <option key={item.id} value={item.id}>{item.name}</option>
                  ))}
                </select>
                <input required maxLength={10} placeholder="Grade name (A+)" value={draft.gradeName} onChange={(event) => setDraft((value) => ({ ...value, gradeName: event.target.value }))} className="rounded-md border bg-background px-3 py-2" />
                <input required type="number" step="0.01" min="0" max="5" placeholder="Grade point" value={draft.gradePoint} onChange={(event) => setDraft((value) => ({ ...value, gradePoint: event.target.value }))} className="rounded-md border bg-background px-3 py-2" />
                <input required type="number" step="0.01" placeholder="Min marks" value={draft.minMarks} onChange={(event) => setDraft((value) => ({ ...value, minMarks: event.target.value }))} className="rounded-md border bg-background px-3 py-2" />
                <input required type="number" step="0.01" placeholder="Max marks" value={draft.maxMarks} onChange={(event) => setDraft((value) => ({ ...value, maxMarks: event.target.value }))} className="rounded-md border bg-background px-3 py-2" />
                <input required type="number" step="0.01" min="0" max="100" placeholder="Min %" value={draft.minPercentage} onChange={(event) => setDraft((value) => ({ ...value, minPercentage: event.target.value }))} className="rounded-md border bg-background px-3 py-2" />
                <input required type="number" step="0.01" min="0" max="100" placeholder="Max %" value={draft.maxPercentage} onChange={(event) => setDraft((value) => ({ ...value, maxPercentage: event.target.value }))} className="rounded-md border bg-background px-3 py-2" />
                <input required type="number" min="0" placeholder="Display order" value={draft.displayOrder} onChange={(event) => setDraft((value) => ({ ...value, displayOrder: event.target.value }))} className="rounded-md border bg-background px-3 py-2" />
                <label className="flex items-center gap-2 text-sm">
                  <input type="checkbox" checked={draft.isFail} onChange={(event) => setDraft((value) => ({ ...value, isFail: event.target.checked }))} />
                  Fail grade
                </label>
                <div className="flex gap-2 md:col-span-2">
                  <Button type="submit" disabled={createGrade.isPending || updateGrade.isPending}>
                    {createGrade.isPending || updateGrade.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                    {editingId ? "Save" : "Create"}
                  </Button>
                  {editingId ? <Button type="button" variant="outline" onClick={() => { setEditingId(null); setDraft(emptyDraft); }}>Cancel</Button> : null}
                </div>
              </form>
            </CardContent>
          </Card>
        ) : null}

        <Card>
          <CardHeader>
            <CardTitle>Grade bands</CardTitle>
            <CardDescription>
              <select value={yearFilter} onChange={(event) => setYearFilter(event.target.value)} className="mt-2 rounded-md border bg-background px-3 py-2">
                <option value="">All years</option>
                {years.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
              </select>
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-2">
            {isPending ? <p className="text-sm text-muted-foreground">Loading…</p> : null}
            {isError ? <p className="text-sm text-destructive">Unable to load grade setup.</p> : null}
            {filtered.map((item) => (
              <div key={item.id} className="rounded-lg border p-3">
                <div className="flex items-start justify-between gap-3">
                  <div>
                    <p className="font-medium">{item.gradeName} · GPA {item.gradePoint}</p>
                    <p className="text-sm text-muted-foreground">
                      {item.academicYearName} · {item.minPercentage}–{item.maxPercentage}% · {item.isActive ? "Active" : "Inactive"}
                      {item.isFail ? " · Fail" : ""}
                    </p>
                  </div>
                  {canManage ? (
                    <div className="flex flex-wrap gap-2">
                      <Button type="button" variant="outline" onClick={() => startEdit(item)}><Pencil className="h-4 w-4" /></Button>
                      <Button type="button" variant="outline" onClick={() => void toggleGrade.mutateAsync({ id: item.id, activate: !item.isActive }).catch((error) => toast.error(getExamErrorMessage(error)))}>
                        {item.isActive ? "Deactivate" : "Activate"}
                      </Button>
                      <Button type="button" variant="outline" onClick={() => {
                        if (!window.confirm("Delete this grade band?")) return;
                        void deleteGrade.mutateAsync(item.id).then(() => toast.success("Deleted.")).catch((error) => toast.error(getExamErrorMessage(error)));
                      }}><Trash2 className="h-4 w-4" /></Button>
                    </div>
                  ) : null}
                </div>
              </div>
            ))}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
