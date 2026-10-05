import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { ArrowLeft, BookOpen, Loader2, Plus, Search, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import {
  useClassSubjects,
  useCreateClassSubjects,
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
  const createAssignments = useCreateClassSubjects();
  const deleteAssignment = useDeleteClassSubject();
  const updateOptional = useUpdateClassSubjectOptional();
  const [classId, setClassId] = useState("");
  const [subjectIds, setSubjectIds] = useState<number[]>([]);
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

  const groupedFiltered = useMemo(() => {
    const groups = new Map<number, { className: string; items: typeof filtered }>();

    filtered.forEach((item) => {
      const className = classes.find((schoolClass) => schoolClass.id === item.classId)?.name ?? `Class #${item.classId}`;
      const group = groups.get(item.classId) ?? { className, items: [] };
      group.items.push(item);
      groups.set(item.classId, group);
    });

    return [...groups.entries()]
      .map(([groupClassId, group]) => ({ classId: groupClassId, ...group }))
      .sort((a, b) => {
        const classA = classes.find((schoolClass) => schoolClass.id === a.classId);
        const classB = classes.find((schoolClass) => schoolClass.id === b.classId);
        return (classA?.displayOrder ?? Number.MAX_SAFE_INTEGER) - (classB?.displayOrder ?? Number.MAX_SAFE_INTEGER)
          || a.className.localeCompare(b.className);
      })
      .map((group) => ({
        ...group,
        items: group.items.sort((a, b) => {
          const subjectA = subjects.find((subject) => subject.id === a.subjectId)?.name ?? "";
          const subjectB = subjects.find((subject) => subject.id === b.subjectId)?.name ?? "";
          return subjectA.localeCompare(subjectB);
        }),
      }));
  }, [filtered, classes, subjects]);

  const assignedPairs = useMemo(
    () => new Set(classSubjects.map((item) => `${item.classId}:${item.subjectId}`)),
    [classSubjects]
  );

  const availableSubjects = useMemo(
    () => subjects.filter((subject) => !assignedPairs.has(`${classId}:${subject.id}`)),
    [assignedPairs, classId, subjects]
  );

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!classId || subjectIds.length === 0) return;

    try {
      const results = await createAssignments.mutateAsync(
        subjectIds.map((subjectId) => ({
          classId: Number(classId),
          subjectId,
          isOptional,
        }))
      );
      const failedSubjectIds = subjectIds.filter(
        (_, index) => results[index].status === "rejected"
      );
      const assignedCount = subjectIds.length - failedSubjectIds.length;

      setSubjectIds(failedSubjectIds);
      if (failedSubjectIds.length === 0) {
        setIsOptional(false);
        toast.success(`${assignedCount} subject${assignedCount === 1 ? "" : "s"} assigned to class.`);
      } else if (assignedCount > 0) {
        toast.error(
          `${assignedCount} subject${assignedCount === 1 ? "" : "s"} assigned; ${failedSubjectIds.length} failed. The failed selections are kept so you can retry.`
        );
      } else {
        toast.error("Could not assign the selected subjects. Please try again.");
      }
    } catch {
      toast.error("Could not assign the selected subjects. Please try again.");
    }
  }

  async function handleOptionalToggle(item: { classId: number; subjectId: number; isOptional: boolean }) {
    try {
      await updateOptional.mutateAsync({ ...item, isOptional: !item.isOptional });
      toast.success("Subject requirement updated.");
    } catch {
      toast.error("Could not update the subject requirement.");
    }
  }

  async function handleRemove(item: { classId: number; subjectId: number; subjectName: string; className: string }) {
    if (!window.confirm(`Remove ${item.subjectName} from ${item.className}?`)) return;

    try {
      await deleteAssignment.mutateAsync({ classId: item.classId, subjectId: item.subjectId });
      toast.success("Subject removed from class.");
    } catch {
      toast.error("Could not remove this subject from the class.");
    }
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
              <CardTitle>Assign subjects</CardTitle>
              <CardDescription>Select a class and add multiple subjects to its curriculum.</CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label htmlFor="class-subject-class" className="mb-1.5 block text-sm font-medium">Class</label>
                  <select id="class-subject-class" required value={classId} onChange={(event) => {
                    setClassId(event.target.value);
                    setSubjectIds([]);
                  }} className="w-full rounded-md border bg-background px-3 py-2.5">
                    <option value="">Select class</option>
                    {classes.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
                  </select>
                </div>
                <fieldset disabled={!classId || isPending || createAssignments.isPending} className="space-y-2">
                  <div className="flex items-center justify-between gap-2">
                    <legend className="text-sm font-medium">Subjects</legend>
                    {availableSubjects.length > 0 ? (
                      <div className="flex gap-3">
                        <button type="button" className="text-xs font-medium text-primary hover:underline" onClick={() => setSubjectIds(availableSubjects.map((subject) => subject.id))}>
                          Select all
                        </button>
                        {subjectIds.length > 0 ? (
                          <button type="button" className="text-xs font-medium text-muted-foreground hover:underline" onClick={() => setSubjectIds([])}>
                            Clear
                          </button>
                        ) : null}
                      </div>
                    ) : null}
                  </div>
                  <div className="max-h-56 space-y-1 overflow-y-auto rounded-md border p-2">
                    {!classId ? (
                      <p className="p-2 text-sm text-muted-foreground">Select a class to see available subjects.</p>
                    ) : isPending ? (
                      <p className="p-2 text-sm text-muted-foreground">Loading assigned subjects…</p>
                    ) : availableSubjects.length === 0 ? (
                      <p className="p-2 text-sm text-muted-foreground">
                        {subjects.length === 0 ? "No subjects are available." : "All subjects are already assigned to this class."}
                      </p>
                    ) : availableSubjects.map((subject) => (
                      <label key={subject.id} className="flex cursor-pointer items-center gap-3 rounded-md px-2 py-2 text-sm hover:bg-muted/60">
                        <input
                          type="checkbox"
                          checked={subjectIds.includes(subject.id)}
                          onChange={(event) => setSubjectIds((current) =>
                            event.target.checked
                              ? [...current, subject.id]
                              : current.filter((id) => id !== subject.id)
                          )}
                          className="h-4 w-4 accent-emerald-600"
                        />
                        <span>{subject.name} ({subject.code})</span>
                      </label>
                    ))}
                  </div>
                  <p className="text-xs text-muted-foreground">
                    {subjectIds.length} subject{subjectIds.length === 1 ? "" : "s"} selected
                  </p>
                </fieldset>
                {classId && isError ? (
                  <p role="alert" className="text-sm text-destructive">Unable to load existing class assignments. Refresh and try again.</p>
                ) : null}
                <label className="flex items-center gap-3 rounded-lg border bg-amber-50/70 p-3 text-sm">
                  <input type="checkbox" checked={isOptional} onChange={(event) => setIsOptional(event.target.checked)} className="h-4 w-4 accent-emerald-600" />
                  <span>
                    <span className="block font-medium">Optional subjects</span>
                    <span className="text-xs text-muted-foreground">Apply this setting to every selected subject.</span>
                  </span>
                </label>
                <Button type="submit" disabled={
                  createAssignments.isPending || isPending || !classId || subjectIds.length === 0
                } className="w-full">
                  {createAssignments.isPending ? <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" /> : <Plus aria-hidden="true" className="mr-2 h-4 w-4" />}
                  {createAssignments.isPending
                    ? "Assigning subjects…"
                    : subjectIds.length > 0
                      ? `Assign ${subjectIds.length} subject${subjectIds.length === 1 ? "" : "s"}`
                      : "Assign subjects"}
                </Button>
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
            {!isPending && !isError && groupedFiltered.length === 0 ? <div className="rounded-lg border border-dashed bg-muted/30 p-8 text-center text-sm text-muted-foreground">No class subjects found. Assign subjects to classes to build the curriculum.</div> : null}
            {!isPending && !isError ? (
              <div className="space-y-4">
                {groupedFiltered.map(({ classId: groupClassId, className, items }) => (
                  <section key={groupClassId} aria-label={`${className} subjects`} className="overflow-hidden rounded-lg border bg-card">
                    <div className="flex items-center justify-between gap-3 border-b bg-muted/40 px-4 py-3">
                      <h3 className="font-semibold">{className}</h3>
                      <span className="rounded-full bg-background px-2.5 py-1 text-xs font-medium text-muted-foreground">
                        {items.length} subject{items.length === 1 ? "" : "s"}
                      </span>
                    </div>
                    <div className="divide-y">
                      {items.map((item) => {
                        const subject = subjects.find((entry) => entry.id === item.subjectId);
                        return (
                          <div key={`${item.classId}-${item.subjectId}`} className="flex flex-wrap items-center justify-between gap-3 px-4 py-3 transition hover:bg-muted/20">
                            <div className="min-w-0">
                              <p className="font-medium">{subject?.name ?? `Subject #${item.subjectId}`}</p>
                              {subject?.code ? <p className="mt-0.5 text-sm text-muted-foreground">{subject.code}</p> : null}
                            </div>
                            <div className="flex items-center gap-2">
                              <span className={`rounded-full px-2.5 py-1 text-xs font-medium ${item.isOptional ? "bg-amber-100 text-amber-900" : "bg-emerald-100 text-emerald-900"}`}>
                                {item.isOptional ? "Optional" : "Required"}
                              </span>
                              {canAssign ? (
                                <Button type="button" variant="outline" disabled={updateOptional.isPending} onClick={() => void handleOptionalToggle(item)}>
                                  Mark {item.isOptional ? "required" : "optional"}
                                </Button>
                              ) : null}
                              {canRemove ? (
                                <Button type="button" variant="outline" className="h-9 w-9 p-0 text-destructive hover:bg-destructive/10" aria-label={`Remove ${subject?.name ?? "subject"} from ${className}`} disabled={deleteAssignment.isPending} onClick={() => void handleRemove({ classId: item.classId, subjectId: item.subjectId, subjectName: subject?.name ?? "this subject", className })}>
                                  <Trash2 aria-hidden="true" className="h-4 w-4" />
                                </Button>
                              ) : null}
                            </div>
                          </div>
                        );
                      })}
                    </div>
                  </section>
                ))}
              </div>
            ) : null}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}