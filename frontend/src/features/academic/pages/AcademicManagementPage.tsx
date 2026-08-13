import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import {
  BookOpen,
  CalendarDays,
  GraduationCap,
  Loader2,
  Plus,
  School,
  ScrollText,
  UserRound,
} from "lucide-react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useAcademicYears, useSchoolClasses, useSections, useSubjects, useTeachers } from "../hooks/useAcademicData";

const sections = [
  { key: "years", label: "Academic Years", icon: CalendarDays, path: "/academic/years" },
  { key: "sessions", label: "Academic Sessions", icon: School, path: "/academic/sessions" },
  { key: "classes", label: "Classes", icon: GraduationCap, path: "/academic/classes" },
  { key: "sections", label: "Sections", icon: ScrollText, path: "/academic/sections" },
  { key: "subjects", label: "Subjects", icon: BookOpen, path: "/academic/subjects" },
  { key: "teachers", label: "Teachers", icon: UserRound, path: "/academic/teachers" },
  { key: "assignments", label: "Teacher Assignments", icon: UserRound, path: "/academic/teacher-assignments" },
];

export function AcademicManagementPage() {
  const [active, setActive] = useState<string>(sections[0].key);
  const { data: years, isPending: yearsPending } = useAcademicYears();
  const { data: classes, isPending: classesPending } = useSchoolClasses();
  const { data: sectionsData, isPending: sectionsPending } = useSections();
  const { data: subjects, isPending: subjectsPending } = useSubjects();
  const { data: teachers, isPending: teachersPending } = useTeachers();

  const summary = useMemo(() => ({
    years: years?.length ?? 0,
    classes: classes?.length ?? 0,
    sections: sectionsData?.length ?? 0,
    subjects: subjects?.length ?? 0,
    teachers: teachers?.length ?? 0,
  }), [years, classes, sectionsData, subjects, teachers]);

  const pending = yearsPending || classesPending || sectionsPending || subjectsPending || teachersPending;

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <Card className="overflow-hidden border-primary/20 bg-gradient-to-br from-primary to-primary/90 text-primary-foreground">
        <CardContent className="flex flex-col gap-4 p-6 sm:flex-row sm:items-center sm:justify-between sm:p-8">
          <div>
            <p className="text-sm uppercase tracking-[0.2em] text-primary-foreground/70">Academic Management</p>
            <h1 className="mt-2 font-display text-2xl font-semibold sm:text-3xl">Manage academic structure</h1>
            <p className="mt-2 max-w-2xl text-sm text-primary-foreground/80">Years, sessions, classes, sections, subjects, and teacher assignments are organized here for quick administration.</p>
          </div>
          <Link to="/academic/years" className="inline-flex items-center justify-center rounded-md bg-background px-4 py-2 text-sm font-medium text-primary transition hover:bg-background/90">
            <Plus aria-hidden="true" className="mr-2 h-4 w-4" />
            Open Academic Setup
          </Link>
        </CardContent>
      </Card>

      {pending ? (
        <div className="flex min-h-48 items-center justify-center rounded-xl border border-dashed bg-card/70 p-8 text-muted-foreground">
          <div className="flex items-center gap-3">
            <Loader2 aria-hidden="true" className="h-5 w-5 animate-spin" />
            Loading academic data…
          </div>
        </div>
      ) : (
        <>
          <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-5">
            <Card>
              <CardHeader><CardTitle className="text-sm font-medium text-muted-foreground">Academic Years</CardTitle></CardHeader>
              <CardContent><div className="text-3xl font-semibold tabular-nums">{summary.years}</div></CardContent>
            </Card>
            <Card>
              <CardHeader><CardTitle className="text-sm font-medium text-muted-foreground">Classes</CardTitle></CardHeader>
              <CardContent><div className="text-3xl font-semibold tabular-nums">{summary.classes}</div></CardContent>
            </Card>
            <Card>
              <CardHeader><CardTitle className="text-sm font-medium text-muted-foreground">Sections</CardTitle></CardHeader>
              <CardContent><div className="text-3xl font-semibold tabular-nums">{summary.sections}</div></CardContent>
            </Card>
            <Card>
              <CardHeader><CardTitle className="text-sm font-medium text-muted-foreground">Subjects</CardTitle></CardHeader>
              <CardContent><div className="text-3xl font-semibold tabular-nums">{summary.subjects}</div></CardContent>
            </Card>
            <Card>
              <CardHeader><CardTitle className="text-sm font-medium text-muted-foreground">Teachers</CardTitle></CardHeader>
              <CardContent><div className="text-3xl font-semibold tabular-nums">{summary.teachers}</div></CardContent>
            </Card>
          </div>

          <div className="grid gap-4 lg:grid-cols-[0.9fr_1.1fr]">
            <Card>
              <CardHeader>
                <CardTitle>Academic Areas</CardTitle>
                <CardDescription>Jump directly to the management area you need.</CardDescription>
              </CardHeader>
              <CardContent className="space-y-2">
                {sections.map((item) => {
                  const Icon = item.icon;
                  return (
                    <button
                      key={item.key}
                      type="button"
                      onClick={() => setActive(item.key)}
                      className={`flex w-full items-center justify-between rounded-lg border px-4 py-3 text-left transition ${active === item.key ? "border-primary bg-primary/5" : "border-border hover:bg-muted/40"}`}
                    >
                      <span className="flex items-center gap-3">
                        <Icon aria-hidden="true" className="h-4 w-4" />
                        {item.label}
                      </span>
                      <span className="text-sm text-muted-foreground">Open</span>
                    </button>
                  );
                })}
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Quick guidance</CardTitle>
                <CardDescription>Each area supports list, create, edit, detail, and delete flows where the backend exposes them.</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="rounded-lg border bg-muted/40 p-4 text-sm text-muted-foreground">
                  {active === "years" && "Manage academic year definitions and mark the current year."}
                  {active === "sessions" && "Academic sessions can be maintained alongside the year setup."}
                  {active === "classes" && "Create and organize school classes and their order."}
                  {active === "sections" && "Assign sections under each class for student grouping."}
                  {active === "subjects" && "Maintain the subject catalog and grading values."}
                  {active === "assignments" && "Link teachers to subjects for classroom planning."}
                </div>
              </CardContent>
            </Card>
          </div>
        </>
      )}
    </div>
  );
}
