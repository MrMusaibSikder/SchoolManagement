import { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Loader2, Pencil, Plus, Search, Trash2, UserRound } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useSchoolClasses, useSections } from "@/features/academic/hooks/useAcademicData";
import { useDeleteStudent, useStudents } from "../hooks/useStudentData";
import type { StudentListFilterState } from "../types/student.types";

export function StudentsPage() {
  const navigate = useNavigate();
  const { data = [], isPending, isError } = useStudents();
  const deleteStudentMutation = useDeleteStudent();
  const { data: classes = [] } = useSchoolClasses();
  const { data: sections = [] } = useSections();
  const [filters, setFilters] = useState<StudentListFilterState>({ search: "", classId: "", sectionId: "", status: "" });
  const [page, setPage] = useState(1);
  const pageSize = 8;

  const filtered = useMemo(() => {
    const search = filters.search.toLowerCase();
    return data.filter((item) => {
      const matchesSearch = [item.fullName, item.admissionNumber, item.rollNo].join(" ").toLowerCase().includes(search);
      const matchesClass = filters.classId ? String(item.classId) === filters.classId : true;
      const matchesSection = filters.sectionId ? String(item.sectionId) === filters.sectionId : true;
      const matchesStatus = filters.status ? filters.status === "active" : true;
      return matchesSearch && matchesClass && matchesSection && matchesStatus;
    });
  }, [data, filters]);

  const paged = useMemo(() => {
    const start = (page - 1) * pageSize;
    return filtered.slice(start, start + pageSize);
  }, [filtered, page]);

  const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize));

  function resetFilters() {
    setFilters({ search: "", classId: "", sectionId: "", status: "" });
    setPage(1);
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Students</h1>
          <p className="text-sm text-muted-foreground">Search, filter, and manage student records.</p>
        </div>
        <Button onClick={() => navigate("/students/new")}>
          <Plus className="mr-2 h-4 w-4" />
          Add Student
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Student directory</CardTitle>
          <CardDescription>Use the filters below to narrow the list by class, section, or search text.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 md:grid-cols-4">
            <div className="relative md:col-span-2">
              <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <input value={filters.search} onChange={(event) => { setFilters((value) => ({ ...value, search: event.target.value })); setPage(1); }} placeholder="Search by name, roll, admission" className="w-full rounded-md border bg-background py-2 pl-9 pr-3" />
            </div>
            <select value={filters.classId} onChange={(event) => { setFilters((value) => ({ ...value, classId: event.target.value })); setPage(1); }} className="rounded-md border bg-background px-3 py-2">
              <option value="">All classes</option>
              {classes.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
            </select>
            <select value={filters.sectionId} onChange={(event) => { setFilters((value) => ({ ...value, sectionId: event.target.value })); setPage(1); }} className="rounded-md border bg-background px-3 py-2">
              <option value="">All sections</option>
              {sections.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
            </select>
          </div>
          <div className="flex flex-wrap items-center gap-3">
            <select value={filters.status} onChange={(event) => { setFilters((value) => ({ ...value, status: event.target.value })); setPage(1); }} className="rounded-md border bg-background px-3 py-2">
              <option value="">All status</option>
              <option value="active">Active</option>
              <option value="inactive">Inactive</option>
            </select>
            <Button type="button" variant="outline" onClick={resetFilters}>Reset</Button>
          </div>

          {isPending ? <div className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading students…</div> : null}
          {isError ? <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load students.</div> : null}
          {!isPending && !isError && filtered.length === 0 ? <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">No students match the current filters.</div> : null}

          <div className="space-y-2">
            {paged.map((student) => (
              <div key={student.id} className="flex flex-col gap-3 rounded-lg border p-4 md:flex-row md:items-center md:justify-between">
                <div className="flex items-center gap-3">
                  <div className="flex h-10 w-10 items-center justify-center rounded-full bg-muted text-muted-foreground">
                    <UserRound className="h-5 w-5" />
                  </div>
                  <div>
                    <p className="font-medium">{student.fullName}</p>
                    <p className="text-sm text-muted-foreground">Admission #{student.admissionNumber} • Roll {student.rollNo}</p>
                  </div>
                </div>
                <div className="flex flex-wrap items-center gap-2">
                  <span className="rounded-full bg-primary/10 px-2.5 py-1 text-xs font-medium text-primary">Class {student.classId} • Sec {student.sectionId}</span>
                  <Button type="button" variant="outline" onClick={() => navigate(`/students/${student.id}`)}>View</Button>
                  <Button type="button" variant="outline" onClick={() => navigate(`/students/${student.id}/edit`)}><Pencil className="h-4 w-4" /></Button>
                  <Button type="button" variant="outline" onClick={() => deleteStudentMutation.mutate(student.id)}><Trash2 className="h-4 w-4" /></Button>
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
