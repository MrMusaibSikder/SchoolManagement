import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, Pencil, Plus, Search, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useCreateSubject, useDeleteSubject, useSubjects, useUpdateSubject } from "../hooks/useAcademicData";
import type { SubjectDto } from "../types/academic.types";

export function SubjectsPage() {
  const { data = [], isPending, isError } = useSubjects();
  const createSubjectMutation = useCreateSubject();
  const updateSubjectMutation = useUpdateSubject();
  const deleteSubjectMutation = useDeleteSubject();
  const [search, setSearch] = useState("");
  const [name, setName] = useState("");
  const [code, setCode] = useState("");
  const [editingId, setEditingId] = useState<number | null>(null);

  const filtered = useMemo(() => data.filter((item) => `${item.name} ${item.code}`.toLowerCase().includes(search.toLowerCase())), [data, search]);

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    const payload = { name, code, fullMarks: 100, passMarks: 40 };
    if (editingId) {
      await updateSubjectMutation.mutateAsync({ id: editingId, payload: { ...payload, id: editingId } });
    } else {
      await createSubjectMutation.mutateAsync(payload);
    }
    setName("");
    setCode("");
    setEditingId(null);
  }

  function startEdit(item: SubjectDto) {
    setEditingId(item.id);
    setName(item.name);
    setCode(item.code);
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Subjects</h1>
          <p className="text-sm text-muted-foreground">Manage curriculum subjects and codes.</p>
        </div>
        <Link to="/academic" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">
          Back to overview
        </Link>
      </div>

      <div className="grid gap-6 lg:grid-cols-[0.9fr_1.1fr]">
        <Card>
          <CardHeader>
            <CardTitle>{editingId ? "Edit subject" : "Create subject"}</CardTitle>
            <CardDescription>Store subject names and a short code.</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="mb-1 block text-sm font-medium">Name</label>
                <input required value={name} onChange={(event) => setName(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Code</label>
                <input required value={code} onChange={(event) => setCode(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div className="flex gap-2">
                <Button type="submit" disabled={createSubjectMutation.isPending || updateSubjectMutation.isPending}>
                  {createSubjectMutation.isPending || updateSubjectMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                  {editingId ? "Save changes" : "Create subject"}
                </Button>
                {editingId ? <Button type="button" variant="outline" onClick={() => { setEditingId(null); setName(""); setCode(""); }}>Cancel</Button> : null}
              </div>
            </form>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Subjects list</CardTitle>
            <CardDescription>Search and manage subjects.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="relative">
              <Search aria-hidden="true" className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search subjects" className="w-full rounded-md border bg-background py-2 pl-9 pr-3" />
            </div>
            {isPending ? <div className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading subjects…</div> : null}
            {isError ? <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load subjects.</div> : null}
            {!isPending && !isError && filtered.length === 0 ? <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">No subjects found.</div> : null}
            <div className="space-y-2">
              {filtered.map((item) => (
                <div key={item.id} className="flex items-center justify-between rounded-lg border p-4">
                  <div>
                    <p className="font-medium">{item.name}</p>
                    <p className="text-sm text-muted-foreground">Code: {item.code}</p>
                  </div>
                  <div className="flex gap-2">
                    <Button type="button" variant="outline" className="h-9 w-9 p-0" onClick={() => startEdit(item)}><Pencil className="h-4 w-4" /></Button>
                    <Button type="button" variant="outline" className="h-9 w-9 p-0" onClick={() => deleteSubjectMutation.mutate(item.id)}><Trash2 className="h-4 w-4" /></Button>
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
