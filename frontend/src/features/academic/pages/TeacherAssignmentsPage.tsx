import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, Plus, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useCreateSubjectTeacher, useDeleteSubjectTeacher, useSubjectTeachers, useSubjects, useTeachers } from "../hooks/useAcademicData";

export function TeacherAssignmentsPage() {
  const { data = [], isPending, isError } = useSubjectTeachers();
  const { data: subjects = [] } = useSubjects();
  const { data: teachers = [] } = useTeachers();
  const createAssignment = useCreateSubjectTeacher();
  const deleteAssignment = useDeleteSubjectTeacher();
  const [subjectId, setSubjectId] = useState("");
  const [teacherId, setTeacherId] = useState("");
  const [search, setSearch] = useState("");

  const filtered = useMemo(() => data.filter((item) => `${item.subjectId} ${item.teacherId}`.toLowerCase().includes(search.toLowerCase())), [data, search]);

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    await createAssignment.mutateAsync({ subjectId: Number(subjectId), teacherId: Number(teacherId) });
    setSubjectId("");
    setTeacherId("");
  }

  const lookupSubject = (id: number) => subjects.find((item) => item.id === id)?.name ?? `Subject #${id}`;
  const lookupTeacher = (id: number) => teachers.find((item) => item.id === id)?.employeeId ?? `Teacher #${id}`;

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Teacher Assignments</h1>
          <p className="text-sm text-muted-foreground">Link teachers with subjects for classroom planning.</p>
        </div>
        <Link to="/academic" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">
          Back to overview
        </Link>
      </div>

      <div className="grid gap-6 lg:grid-cols-[0.9fr_1.1fr]">
        <Card>
          <CardHeader>
            <CardTitle>Create assignment</CardTitle>
            <CardDescription>Select a subject and the teacher who should teach it.</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="mb-1 block text-sm font-medium">Subject</label>
                <select required value={subjectId} onChange={(event) => setSubjectId(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2">
                  <option value="">Select subject</option>
                  {subjects.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
                </select>
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Teacher</label>
                <select required value={teacherId} onChange={(event) => setTeacherId(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2">
                  <option value="">Select teacher</option>
                  {teachers.map((item) => <option key={item.id} value={item.id}>Teacher #{item.employeeId}</option>)}
                </select>
              </div>
              <Button type="submit" disabled={createAssignment.isPending}>
                {createAssignment.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                Create assignment
              </Button>
            </form>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Assignments list</CardTitle>
            <CardDescription>Review and remove subject-teacher assignments.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search assignments" className="w-full rounded-md border bg-background px-3 py-2" />
            {isPending ? <div className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading assignments…</div> : null}
            {isError ? <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load assignments.</div> : null}
            {!isPending && !isError && filtered.length === 0 ? <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">No assignments found.</div> : null}
            <div className="space-y-2">
              {filtered.map((item) => (
                <div key={`${item.subjectId}-${item.teacherId}`} className="flex items-center justify-between rounded-lg border p-4">
                  <div>
                    <p className="font-medium">{lookupSubject(item.subjectId)}</p>
                    <p className="text-sm text-muted-foreground">{lookupTeacher(item.teacherId)}</p>
                  </div>
                  <Button type="button" variant="outline" className="h-9 w-9 p-0" onClick={() => deleteAssignment.mutate({ subjectId: item.subjectId, teacherId: item.teacherId })}><Trash2 className="h-4 w-4" /></Button>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
