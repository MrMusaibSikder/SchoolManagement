import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { ArrowLeft, BookOpen, Loader2, Plus, Search, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import {
  useClassSubjects,
  useCreateClassSubject,
  useDeleteClassSubject,
  useSchoolClasses,
  useSubjects,
  useUpdateClassSubjectOptional,
} from "../hooks/useAcademicData";

export function ClassSubjectsPage() {
  const { hasPermission } = usePermissions();
  const canView = hasPermission(Permission.ClassSubjectView);
  const canAssign = hasPermission(Permission.ClassSubjectAssign);
  const canRemove = hasPermission(Permission.ClassSubjectRemove);
  const { data: classSubjects = [], isPending, isError } = useClassSubjects();
  const { data: classes = [] } = useSchoolClasses();
  const { data: subjects = [] } = useSubjects();
  const createAssignment = useCreateClassSubject();
  const deleteAssignment = useDeleteClassSubject();
  const updateOptional = useUpdateClassSubjectOptional();
  const [classId, setClassId] = useState("");
  const [subjectId, setSubjectId] = useState("");
  const [isOptional, setIsOptional] = useState(false);
  const [search, setSearch] = useState("");

  const filtered = useMemo(
    () => classSubjects.filter((item) => {
      const className = classes.find((schoolClass) => schoolClass.id === item.classId)?.name ?? "";
      const subjectName = subjects.find((subject) => subject.id === item.subjectId)?.name ?? "";
      return `${className} ${subjectName}`.toLowerCase().includes(search.toLowerCase());
    }),
    [classSubjects, classes, subjects, search]
  );

  const assignedPairs = useMemo(
    () => new Set(classSubjects.map((item) => `${item.classId}:${item.subjectId}`)),
    [classSubjects]
  );

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!classId || !subjectId) return;

    await createAssignment.mutateAsync({
      classId: Number(classId),
      subjectId: Number(subjectId),
      isOptional,
    });
    setSubjectId("");
    setIsOptional(false);
  }

  if (!canView) {
    return (
      <Card className="mx-auto max-w-7xl">
        <CardContent className="p-6 text-sm text-destructive">
          You do not have permission to view class subjects.
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-4 rounded-xl border border-emerald-200 bg-gradient-to-r from-emerald-50 via-white to-amber-50 p-5 sm:flex-row sm:items-center sm:justify-between sm:p-7">
        <div className="flex items-start gap-4">
          <span className="flex h-11 w-11 shrink-0 items-center justify-center rounded-lg bg-emerald-600 text-white shadow-sm">
            <BookOpen aria-hidden="true" className="h-5 w-5" />
          </span>
          <div>
            <p className="text-xs font-semibold uppercase tracking-wider text-emerald-800">Curriculum setup</p>
            <h1 className="mt-1 font-display text-2xl font-semibold text-foreground">Class Subjects</h1>
            <p className="mt-1 text-sm text-muted-foreground">Choose which subjects belong to each class.</p>
          </div>
        </div>
        <Link to="/academic" className="inline-flex items-center gap-2 self-start rounded-md border bg-white px-3 py-2 text-sm font-medium transition hover:bg-emerald-50 sm:self-center">
          <ArrowLeft aria-hidden="true" className="h-4 w-4" />
          Academic setup
        </Link>
      </div>

      <div className="grid gap-6 lg:grid-cols-[minmax(280px,0.8fr)_minmax(0,1.2fr)]">
        {canAssign ? (
          <Card className="h-fit border-t-4 border-t-amber-400">
            <CardHeader>
              <CardTitle>Assign a subject</CardTitle>
              <CardDescription>Select a class and add one subject to its curriculum.</CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label htmlFor="class-subject-class" className="mb-1.5 block text-sm font-medium">Class</label>
                  <select id="class-subject-class" required value={classId} onChange={(event) => setClassId(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2.5">
                    <option value="">Select class</option>
                    {classes.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
                  </select>
                </div>
                <div>
                  <label htmlFor="class-subject-subject" className="mb-1.5 block text-sm font-medium">Subject</label>
                  <select id="class-subject-subject" required value={subjectId} onChange={(event) => setSubjectId(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2.5">
                    <option value="">Select subject</option>
                    {subjects.map((item) => <option key={item.id} value={item.id}>{item.name} ({item.code})</option>)}
                  </select>
                </div>
                <label className="flex items-center gap-3 rounded-lg border bg-amber-50/70 p-3 text-sm">
                  <input type="checkbox" checked={isOptional} onChange={(event) => setIsOptional(event.target.checked)} className="h-4 w-4 accent-emerald-600" />
                  <span>
                    <span className="block font-medium">Optional subject</span>
                    <span className="text-xs text-muted-foreground">Mark this subject as optional for the class.</span>
                  </span>
                </label>
                <Button type="submit" disabled={
                  createAssignment.isPending || !classId || !subjectId || assignedPairs.has(`${classId}:${subjectId}`)
                } className="w-full">
                  {createAssignment.isPending ? <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" /> : <Plus aria-hidden="true" className="mr-2 h-4 w-4" />}
                  Assign subject
                </Button>
                {classId && subjectId && assignedPairs.has(`${classId}:${subjectId}`) ? (
                  <p className="text-xs text-amber-800">This subject is already assigned to the selected class.</p>
                ) : null}
              </form>
            </CardContent>
          </Card>
        ) : null}

        <Card className="border-t-4 border-t-sky-500">
          <CardHeader>
            <CardTitle>Assigned subjects</CardTitle>
            <CardDescription>{classSubjects.length} class-subject assignment{classSubjects.length === 1 ? "" : "s"}</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="relative">
              <Search aria-hidden="true" className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search by class or subject" className="w-full rounded-md border bg-background py-2.5 pl-9 pr-3" />
            </div>
            {isPending ? <div className="flex items-center justify-center py-10 text-sm text-muted-foreground"><Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />Loading class subjects…</div> : null}
            {isError ? <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load class subjects.</div> : null}
            {!isPending && !isError && filtered.length === 0 ? <div className="rounded-lg border border-dashed bg-muted/30 p-8 text-center text-sm text-muted-foreground">No class subjects found. Assign subjects to classes to build the curriculum.</div> : null}
            {!isPending && !isError ? (
              <div className="space-y-2">
                {filtered.map((item) => {
                  const className = classes.find((schoolClass) => schoolClass.id === item.classId)?.name ?? `Class #${item.classId}`;
                  const subject = subjects.find((entry) => entry.id === item.subjectId);
                  return (
                    <div key={`${item.classId}-${item.subjectId}`} className="flex flex-wrap items-center justify-between gap-3 rounded-lg border bg-card p-4 transition hover:border-primary/40 hover:shadow-sm">
                      <div className="min-w-0">
                        <p className="font-medium">{className}</p>
                        <p className="mt-0.5 text-sm text-muted-foreground">{subject?.name ?? `Subject #${item.subjectId}`}{subject?.code ? ` · ${subject.code}` : ""}</p>
                      </div>
                      <div className="flex items-center gap-2">
                        <span className={`rounded-full px-2.5 py-1 text-xs font-medium ${item.isOptional ? "bg-amber-100 text-amber-900" : "bg-emerald-100 text-emerald-900"}`}>
                          {item.isOptional ? "Optional" : "Required"}
                        </span>
                        {canAssign ? (
                          <Button type="button" variant="outline" disabled={updateOptional.isPending} onClick={() => updateOptional.mutate({ classId: item.classId, subjectId: item.subjectId, isOptional: !item.isOptional })}>
                            Mark {item.isOptional ? "required" : "optional"}
                          </Button>
                        ) : null}
                        {canRemove ? (
                          <Button type="button" variant="outline" className="h-9 w-9 p-0 text-destructive hover:bg-destructive/10" aria-label={`Remove ${subject?.name ?? "subject"} from ${className}`} disabled={deleteAssignment.isPending} onClick={() => deleteAssignment.mutate({ classId: item.classId, subjectId: item.subjectId })}>
                            <Trash2 aria-hidden="true" className="h-4 w-4" />
                          </Button>
                        ) : null}
                      </div>
                    </div>
                  );
                })}
              </div>
            ) : null}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}