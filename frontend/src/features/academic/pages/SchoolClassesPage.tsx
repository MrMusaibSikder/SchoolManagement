import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, Pencil, Plus, Search, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useCreateSchoolClass, useDeleteSchoolClass, useSchoolClasses, useUpdateSchoolClass } from "../hooks/useAcademicData";
import type { SchoolClassDto } from "../types/academic.types";

export function SchoolClassesPage() {
  const { data = [], isPending, isError } = useSchoolClasses();
  const createClass = useCreateSchoolClass();
  const updateClass = useUpdateSchoolClass();
  const deleteClass = useDeleteSchoolClass();
  const [search, setSearch] = useState("");
  const [name, setName] = useState("");
  const [displayOrder, setDisplayOrder] = useState("1");
  const [editingId, setEditingId] = useState<number | null>(null);

  const filtered = useMemo(() => data.filter((item) => `${item.name} ${item.displayOrder}`.toLowerCase().includes(search.toLowerCase())), [data, search]);

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    const payload = { name, displayOrder: Number(displayOrder) };
    if (editingId) {
      await updateClass.mutateAsync({ id: editingId, payload: { ...payload, id: editingId } });
    } else {
      await createClass.mutateAsync(payload);
    }
    setName("");
    setDisplayOrder("1");
    setEditingId(null);
  }

  function startEdit(item: SchoolClassDto) {
    setEditingId(item.id);
    setName(item.name);
    setDisplayOrder(String(item.displayOrder));
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Classes</h1>
          <p className="text-sm text-muted-foreground">Organize class names and display order.</p>
        </div>
        <Link to="/academic" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">
          Back to overview
        </Link>
      </div>

      <div className="grid gap-6 lg:grid-cols-[0.9fr_1.1fr]">
        <Card>
          <CardHeader>
            <CardTitle>{editingId ? "Edit class" : "Create class"}</CardTitle>
            <CardDescription>Use the display order to control how classes appear in lists.</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="mb-1 block text-sm font-medium">Name</label>
                <input required value={name} onChange={(event) => setName(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Display order</label>
                <input type="number" required value={displayOrder} onChange={(event) => setDisplayOrder(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div className="flex gap-2">
                <Button type="submit" disabled={createClass.isPending || updateClass.isPending}>
                  {createClass.isPending || updateClass.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                  {editingId ? "Save changes" : "Create class"}
                </Button>
                {editingId ? <Button type="button" variant="outline" onClick={() => { setEditingId(null); setName(""); setDisplayOrder("1"); }}>Cancel</Button> : null}
              </div>
            </form>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Classes list</CardTitle>
            <CardDescription>Search and manage classes.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="relative">
              <Search aria-hidden="true" className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search classes" className="w-full rounded-md border bg-background py-2 pl-9 pr-3" />
            </div>
            {isPending ? <div className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading classes…</div> : null}
            {isError ? <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load classes.</div> : null}
            {!isPending && !isError && filtered.length === 0 ? <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">No classes found.</div> : null}
            <div className="space-y-2">
              {filtered.map((item) => (
                <div key={item.id} className="flex items-center justify-between rounded-lg border p-4">
                  <div>
                    <p className="font-medium">{item.name}</p>
                    <p className="text-sm text-muted-foreground">Display order: {item.displayOrder}</p>
                  </div>
                  <div className="flex gap-2">
                    <Button type="button" variant="outline" className="h-9 w-9 p-0" onClick={() => startEdit(item)}><Pencil className="h-4 w-4" /></Button>
                    <Button type="button" variant="outline" className="h-9 w-9 p-0" onClick={() => deleteClass.mutate(item.id)}><Trash2 className="h-4 w-4" /></Button>
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
