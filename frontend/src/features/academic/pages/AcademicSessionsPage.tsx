import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, Pencil, Plus, Search, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useAcademicSessions, useCreateAcademicSession, useDeleteAcademicSession, useUpdateAcademicSession } from "../hooks/useAcademicData";
import type { AcademicSessionDto } from "../types/academic.types";

export function AcademicSessionsPage() {
  const { data = [], isPending, isError } = useAcademicSessions();
  const createSession = useCreateAcademicSession();
  const updateSession = useUpdateAcademicSession();
  const deleteSession = useDeleteAcademicSession();
  const [search, setSearch] = useState("");
  const [draft, setDraft] = useState({ name: "", startDate: "", endDate: "", isCurrent: false });
  const [editingId, setEditingId] = useState<number | null>(null);

  const filtered = useMemo(() => data.filter((item) => [item.name, item.startDate, item.endDate].join(" ").toLowerCase().includes(search.toLowerCase())), [data, search]);

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    const payload = { name: draft.name, startDate: draft.startDate, endDate: draft.endDate, isCurrent: draft.isCurrent };
    if (editingId) {
      await updateSession.mutateAsync({ id: editingId, payload: { ...payload, id: editingId } });
    } else {
      await createSession.mutateAsync(payload);
    }
    setDraft({ name: "", startDate: "", endDate: "", isCurrent: false });
    setEditingId(null);
  }

  function startEdit(item: AcademicSessionDto) {
    setEditingId(item.id);
    setDraft({ name: item.name, startDate: item.startDate?.slice(0, 10) ?? "", endDate: item.endDate?.slice(0, 10) ?? "", isCurrent: item.isCurrent });
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Academic Sessions</h1>
          <p className="text-sm text-muted-foreground">Manage yearly academic sessions and mark the current term.</p>
        </div>
        <Link to="/academic" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">
          Back to overview
        </Link>
      </div>

      <div className="grid gap-6 lg:grid-cols-[0.9fr_1.1fr]">
        <Card>
          <CardHeader>
            <CardTitle>{editingId ? "Edit session" : "Create session"}</CardTitle>
            <CardDescription>Use sessions to track academic periods in the school calendar.</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="mb-1 block text-sm font-medium">Name</label>
                <input required value={draft.name} onChange={(event) => setDraft((value) => ({ ...value, name: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div className="grid gap-4 md:grid-cols-2">
                <div>
                  <label className="mb-1 block text-sm font-medium">Start date</label>
                  <input type="date" required value={draft.startDate} onChange={(event) => setDraft((value) => ({ ...value, startDate: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
                </div>
                <div>
                  <label className="mb-1 block text-sm font-medium">End date</label>
                  <input type="date" required value={draft.endDate} onChange={(event) => setDraft((value) => ({ ...value, endDate: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
                </div>
              </div>
              <label className="flex items-center gap-2 text-sm">
                <input type="checkbox" checked={draft.isCurrent} onChange={(event) => setDraft((value) => ({ ...value, isCurrent: event.target.checked }))} />
                Mark as current session
              </label>
              <div className="flex gap-2">
                <Button type="submit" disabled={createSession.isPending || updateSession.isPending}>
                  {createSession.isPending || updateSession.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                  {editingId ? "Save changes" : "Create session"}
                </Button>
                {editingId ? <Button type="button" variant="outline" onClick={() => { setEditingId(null); setDraft({ name: "", startDate: "", endDate: "", isCurrent: false }); }}>Cancel</Button> : null}
              </div>
            </form>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Sessions list</CardTitle>
            <CardDescription>Search and manage the academic sessions.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="relative">
              <Search aria-hidden="true" className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search sessions" className="w-full rounded-md border bg-background py-2 pl-9 pr-3" />
            </div>
            {isPending ? <div className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading sessions…</div> : null}
            {isError ? <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load sessions.</div> : null}
            {!isPending && !isError && filtered.length === 0 ? <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">No sessions found.</div> : null}
            <div className="space-y-2">
              {filtered.map((item) => (
                <div key={item.id} className="flex items-center justify-between rounded-lg border p-4">
                  <div>
                    <p className="font-medium">{item.name}</p>
                    <p className="text-sm text-muted-foreground">{item.startDate?.slice(0, 10)} → {item.endDate?.slice(0, 10)}</p>
                  </div>
                  <div className="flex items-center gap-2">
                    {item.isCurrent ? <span className="rounded-full bg-primary/10 px-2 py-1 text-xs font-medium text-primary">Current</span> : null}
                    <Button type="button" variant="outline" className="h-9 w-9 p-0" onClick={() => startEdit(item)}><Pencil className="h-4 w-4" /></Button>
                    <Button type="button" variant="outline" className="h-9 w-9 p-0" onClick={() => deleteSession.mutate(item.id)}><Trash2 className="h-4 w-4" /></Button>
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
