import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, Pencil, Plus, Search, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useCreateSection, useDeleteSection, useSchoolClasses, useSections, useUpdateSection } from "../hooks/useAcademicData";
import type { SectionDto } from "../types/academic.types";

export function SectionsPage() {
  const { data = [], isPending, isError } = useSections();
  const { data: classes = [] } = useSchoolClasses();
  const createSectionMutation = useCreateSection();
  const updateSectionMutation = useUpdateSection();
  const deleteSectionMutation = useDeleteSection();
  const [search, setSearch] = useState("");
  const [name, setName] = useState("");
  const [classId, setClassId] = useState("");
  const [editingId, setEditingId] = useState<number | null>(null);

  const filtered = useMemo(() => data.filter((item) => `${item.name} ${item.classId}`.toLowerCase().includes(search.toLowerCase())), [data, search]);

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    const payload = { name, classId: Number(classId) };
    if (editingId) {
      await updateSectionMutation.mutateAsync({ id: editingId, payload: { ...payload, id: editingId } });
    } else {
      await createSectionMutation.mutateAsync(payload);
    }
    setName("");
    setClassId("");
    setEditingId(null);
  }

  function startEdit(item: SectionDto) {
    setEditingId(item.id);
    setName(item.name);
    setClassId(String(item.classId));
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Sections</h1>
          <p className="text-sm text-muted-foreground">Create sections under each class.</p>
        </div>
        <Link to="/academic" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">
          Back to overview
        </Link>
      </div>

      <div className="grid gap-6 lg:grid-cols-[0.9fr_1.1fr]">
        <Card>
          <CardHeader>
            <CardTitle>{editingId ? "Edit section" : "Create section"}</CardTitle>
            <CardDescription>Select the class that owns this section.</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="mb-1 block text-sm font-medium">Name</label>
                <input required value={name} onChange={(event) => setName(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Class</label>
                <select required value={classId} onChange={(event) => setClassId(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2">
                  <option value="">Select class</option>
                  {classes.map((item) => (<option key={item.id} value={item.id}>{item.name}</option>))}
                </select>
              </div>
              <div className="flex gap-2">
                <Button type="submit" disabled={createSectionMutation.isPending || updateSectionMutation.isPending}>
                  {createSectionMutation.isPending || updateSectionMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                  {editingId ? "Save changes" : "Create section"}
                </Button>
                {editingId ? <Button type="button" variant="outline" onClick={() => { setEditingId(null); setName(""); setClassId(""); }}>Cancel</Button> : null}
              </div>
            </form>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Sections list</CardTitle>
            <CardDescription>Search and manage sections.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="relative">
              <Search aria-hidden="true" className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search sections" className="w-full rounded-md border bg-background py-2 pl-9 pr-3" />
            </div>
            {isPending ? <div className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading sections…</div> : null}
            {isError ? <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load sections.</div> : null}
            {!isPending && !isError && filtered.length === 0 ? <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">No sections found.</div> : null}
            <div className="space-y-2">
              {filtered.map((item) => (
                <div key={item.id} className="flex items-center justify-between rounded-lg border p-4">
                  <div>
                    <p className="font-medium">{item.name}</p>
                    <p className="text-sm text-muted-foreground">Class ID: {item.classId}</p>
                  </div>
                  <div className="flex gap-2">
                    <Button type="button" variant="outline" className="h-9 w-9 p-0" onClick={() => startEdit(item)}><Pencil className="h-4 w-4" /></Button>
                    <Button type="button" variant="outline" className="h-9 w-9 p-0" onClick={() => deleteSectionMutation.mutate(item.id)}><Trash2 className="h-4 w-4" /></Button>
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
