import { useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, Pencil, Plus, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { getExamErrorMessage } from "../api/exam.api";
import {
  useCreateExamType,
  useDeleteExamType,
  useExamTypes,
  useUpdateExamType,
} from "../hooks/useExamData";

export function ExamTypesPage() {
  const { hasPermission } = usePermissions();
  const { data = [], isPending, isError } = useExamTypes();
  const createType = useCreateExamType();
  const updateType = useUpdateExamType();
  const deleteType = useDeleteExamType();
  const [name, setName] = useState("");
  const [editingId, setEditingId] = useState<number | null>(null);
  const canCreate = hasPermission(Permission.ExamTypeCreate);
  const canEdit = hasPermission(Permission.ExamTypeEdit);
  const canDelete = hasPermission(Permission.ExamTypeDelete);

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    const trimmed = name.trim();
    if (!trimmed) return;
    try {
      if (editingId) {
        await updateType.mutateAsync({ id: editingId, name: trimmed });
        toast.success("Exam type updated.");
      } else {
        await createType.mutateAsync(trimmed);
        toast.success("Exam type created.");
      }
      setName("");
      setEditingId(null);
    } catch (error) {
      toast.error(getExamErrorMessage(error));
    }
  }

  async function handleDelete(id: number) {
    if (!window.confirm("Delete this exam type? It cannot be deleted if exams still use it.")) return;
    try {
      await deleteType.mutateAsync(id);
      toast.success("Exam type deleted.");
    } catch (error) {
      toast.error(getExamErrorMessage(error));
    }
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <Header title="Exam types" subtitle="Unique names such as Term, Half-Yearly, or Final." />
      <div className="grid gap-6 lg:grid-cols-[0.9fr_1.1fr]">
        {(canCreate || (canEdit && editingId)) && (
          <Card>
            <CardHeader>
              <CardTitle>{editingId ? "Edit type" : "Create type"}</CardTitle>
              <CardDescription>Names must be unique.</CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={(event) => void handleSubmit(event)} className="space-y-4">
                <input
                  required
                  maxLength={100}
                  value={name}
                  onChange={(event) => setName(event.target.value)}
                  className="w-full rounded-md border bg-background px-3 py-2"
                  placeholder="Exam type name"
                />
                <div className="flex gap-2">
                  <Button type="submit" disabled={createType.isPending || updateType.isPending}>
                    {createType.isPending || updateType.isPending ? (
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    ) : (
                      <Plus className="mr-2 h-4 w-4" />
                    )}
                    {editingId ? "Save" : "Create"}
                  </Button>
                  {editingId ? (
                    <Button type="button" variant="outline" onClick={() => { setEditingId(null); setName(""); }}>
                      Cancel
                    </Button>
                  ) : null}
                </div>
              </form>
            </CardContent>
          </Card>
        )}
        <Card>
          <CardHeader>
            <CardTitle>Types</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            {isPending ? <p className="text-sm text-muted-foreground">Loading…</p> : null}
            {isError ? <p className="text-sm text-destructive">Unable to load exam types.</p> : null}
            {!isPending && data.length === 0 ? (
              <p className="text-sm text-muted-foreground">No exam types yet.</p>
            ) : null}
            {data.map((item) => (
              <div key={item.id} className="flex items-center justify-between rounded-lg border p-3">
                <p className="font-medium">{item.name}</p>
                <div className="flex gap-2">
                  {canEdit ? (
                    <Button type="button" variant="outline" onClick={() => { setEditingId(item.id); setName(item.name); }}>
                      <Pencil className="h-4 w-4" />
                    </Button>
                  ) : null}
                  {canDelete ? (
                    <Button type="button" variant="outline" onClick={() => void handleDelete(item.id)}>
                      <Trash2 className="h-4 w-4" />
                    </Button>
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

function Header({ title, subtitle }: { title: string; subtitle: string }) {
  return (
    <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
      <div>
        <h1 className="font-display text-2xl font-semibold">{title}</h1>
        <p className="text-sm text-muted-foreground">{subtitle}</p>
      </div>
      <Link to="/exams" className="inline-flex items-center justify-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent">
        Back
      </Link>
    </div>
  );
}
