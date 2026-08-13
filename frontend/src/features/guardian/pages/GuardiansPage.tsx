import { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Loader2, Pencil, Plus, Search, Trash2, UserRound } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useAuth } from "@/features/auth/hooks/useAuth";
import { useDeleteGuardian, useGuardians } from "../hooks/useGuardianData";

export function GuardiansPage() {
  const navigate = useNavigate();
  const { session } = useAuth();
  const { data = [], isPending, isError } = useGuardians();
  const deleteGuardianMutation = useDeleteGuardian();
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState("all");
  const [sortBy, setSortBy] = useState("name");
  const [page, setPage] = useState(1);
  const pageSize = 8;

  const canManage = session?.roles?.some((role) => role === "GuardianCreate" || role === "GuardianEdit" || role === "GuardianDelete" || role === "Admin" || role === "SuperAdmin" || role.toLowerCase().includes("admin")) ?? false;

  const filtered = useMemo(() => {
    const value = search.toLowerCase();
    return data.filter((guardian) => {
      const text = [guardian.fullName, guardian.phoneNumber, guardian.email, guardian.address, guardian.occupation].filter(Boolean).join(" ").toLowerCase();
      const matchesSearch = text.includes(value);
      const matchesFilter = filter === "all" ? true : filter === "email" ? Boolean(guardian.email) : !guardian.email;
      return matchesSearch && matchesFilter;
    }).sort((a, b) => {
      if (sortBy === "phone") return a.phoneNumber.localeCompare(b.phoneNumber);
      if (sortBy === "email") return (a.email ?? "").localeCompare(b.email ?? "");
      return a.fullName.localeCompare(b.fullName);
    });
  }, [data, filter, search, sortBy]);

  const paged = useMemo(() => {
    const start = (page - 1) * pageSize;
    return filtered.slice(start, start + pageSize);
  }, [filtered, page]);

  const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize));

  function handleDelete(guardianId: number) {
    if (window.confirm("Delete this guardian record?")) {
      deleteGuardianMutation.mutate(guardianId);
    }
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Guardians</h1>
          <p className="text-sm text-muted-foreground">Search, review, and manage guardian contacts and student links.</p>
        </div>
        <Button onClick={() => navigate("/guardians/new")} disabled={!canManage}>
          <Plus className="mr-2 h-4 w-4" />
          Add Guardian
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Guardian directory</CardTitle>
          <CardDescription>Use the search box to find guardians by name, phone, or email.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 md:grid-cols-[1.3fr_0.7fr_0.7fr]">
            <div className="relative">
              <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <input value={search} onChange={(event) => { setSearch(event.target.value); setPage(1); }} placeholder="Search guardians" className="w-full rounded-md border bg-background py-2 pl-9 pr-3" />
            </div>
            <select value={filter} onChange={(event) => { setFilter(event.target.value); setPage(1); }} className="rounded-md border bg-background px-3 py-2">
              <option value="all">All contacts</option>
              <option value="email">With email</option>
              <option value="no-email">Without email</option>
            </select>
            <select value={sortBy} onChange={(event) => { setSortBy(event.target.value); setPage(1); }} className="rounded-md border bg-background px-3 py-2">
              <option value="name">Sort by name</option>
              <option value="phone">Sort by phone</option>
              <option value="email">Sort by email</option>
            </select>
          </div>

          {isPending ? <div className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading guardians…</div> : null}
          {isError ? <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load guardians.</div> : null}
          {!isPending && !isError && filtered.length === 0 ? <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">No guardians match the current search.</div> : null}

          <div className="space-y-2">
            {paged.map((guardian) => (
              <div key={guardian.id} className="flex flex-col gap-3 rounded-lg border p-4 md:flex-row md:items-center md:justify-between">
                <div className="flex items-center gap-3">
                  <div className="flex h-10 w-10 items-center justify-center rounded-full bg-muted text-muted-foreground">
                    <UserRound className="h-5 w-5" />
                  </div>
                  <div>
                    <p className="font-medium">{guardian.fullName}</p>
                    <p className="text-sm text-muted-foreground">{guardian.phoneNumber} • {guardian.email ?? "No email"}</p>
                  </div>
                </div>
                <div className="flex flex-wrap items-center gap-2">
                  <Button type="button" variant="outline" onClick={() => navigate(`/guardians/${guardian.id}`)}>View</Button>
                  <Button type="button" variant="outline" onClick={() => navigate(`/guardians/${guardian.id}/edit`)} disabled={!canManage}><Pencil className="h-4 w-4" /></Button>
                  <Button type="button" variant="outline" onClick={() => handleDelete(guardian.id)} disabled={!canManage}><Trash2 className="h-4 w-4" /></Button>
                </div>
              </div>
            ))}
          </div>

          <div className="flex items-center justify-between gap-3 pt-2">
            <p className="text-sm text-muted-foreground">Page {page} of {totalPages}</p>
            <div className="flex gap-2">
              <Button type="button" variant="outline" disabled={page === 1} onClick={() => setPage((value) => Math.max(1, value - 1))}>Previous</Button>
              <Button type="button" variant="outline" disabled={page === totalPages} onClick={() => setPage((value) => Math.min(totalPages, value + 1))}>Next</Button>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
