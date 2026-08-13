import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, Pencil, Plus, Search, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useCreateTeacher, useDeleteTeacher, useTeachers, useUpdateTeacher } from "../hooks/useAcademicData";
import type { TeacherDto } from "../types/academic.types";

export function TeachersPage() {
  const { data = [], isPending, isError } = useTeachers();
  const createTeacherMutation = useCreateTeacher();
  const updateTeacherMutation = useUpdateTeacher();
  const deleteTeacherMutation = useDeleteTeacher();
  const [search, setSearch] = useState("");
  const [employeeId, setEmployeeId] = useState("");
  const [qualification, setQualification] = useState("");
  const [specialization, setSpecialization] = useState("");
  const [editingId, setEditingId] = useState<number | null>(null);

  const filtered = useMemo(() => data.filter((item) => [item.employeeId, item.qualification, item.specialization].filter(Boolean).join(" ").toLowerCase().includes(search.toLowerCase())), [data, search]);

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    const payload = { employeeId: Number(employeeId), qualification, specialization };
    if (editingId) {
      await updateTeacherMutation.mutateAsync({ id: editingId, payload: { ...payload, id: editingId } });
    } else {
      await createTeacherMutation.mutateAsync(payload);
    }
    setEmployeeId("");
    setQualification("");
    setSpecialization("");
    setEditingId(null);
  }

  function startEdit(item: TeacherDto) {
    setEditingId(item.id);
    setEmployeeId(String(item.employeeId));
    setQualification(item.qualification ?? "");
    setSpecialization(item.specialization ?? "");
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Teachers</h1>
          <p className="text-sm text-muted-foreground">Maintain teacher contact records.</p>
        </div>
        <Link to="/academic" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">
          Back to overview
        </Link>
      </div>

      <div className="grid gap-6 lg:grid-cols-[0.9fr_1.1fr]">
        <Card>
          <CardHeader>
            <CardTitle>{editingId ? "Edit teacher" : "Create teacher"}</CardTitle>
            <CardDescription>Enter teacher contact details.</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="mb-1 block text-sm font-medium">Employee ID</label>
                <input type="number" required value={employeeId} onChange={(event) => setEmployeeId(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Qualification</label>
                <input value={qualification} onChange={(event) => setQualification(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Specialization</label>
                <input value={specialization} onChange={(event) => setSpecialization(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div className="flex gap-2">
                <Button type="submit" disabled={createTeacherMutation.isPending || updateTeacherMutation.isPending}>
                  {createTeacherMutation.isPending || updateTeacherMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                  {editingId ? "Save changes" : "Create teacher"}
                </Button>
                {editingId ? <Button type="button" variant="outline" onClick={() => { setEditingId(null); setEmployeeId(""); setQualification(""); setSpecialization(""); }}>Cancel</Button> : null}
              </div>
            </form>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Teachers list</CardTitle>
            <CardDescription>Search and manage teachers.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="relative">
              <Search aria-hidden="true" className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search teachers" className="w-full rounded-md border bg-background py-2 pl-9 pr-3" />
            </div>
            {isPending ? <div className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading teachers…</div> : null}
            {isError ? <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load teachers.</div> : null}
            {!isPending && !isError && filtered.length === 0 ? <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">No teachers found.</div> : null}
            <div className="space-y-2">
              {filtered.map((item) => (
                <div key={item.id} className="flex items-center justify-between rounded-lg border p-4">
                  <div>
                    <p className="font-medium">Teacher #{item.employeeId}</p>
                    <p className="text-sm text-muted-foreground">{item.qualification ?? "Qualification pending"} · {item.specialization ?? "Specialization pending"}</p>
                  </div>
                  <div className="flex gap-2">
                    <Button type="button" variant="outline" className="h-9 w-9 p-0" onClick={() => startEdit(item)}><Pencil className="h-4 w-4" /></Button>
                    <Button type="button" variant="outline" className="h-9 w-9 p-0" onClick={() => deleteTeacherMutation.mutate(item.id)}><Trash2 className="h-4 w-4" /></Button>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
