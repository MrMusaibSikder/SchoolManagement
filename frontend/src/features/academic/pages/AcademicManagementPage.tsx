import { useMemo } from "react";
import { Link } from "react-router-dom";
import {
  BookOpen,
  CalendarDays,
  GraduationCap,
  Link2,
  Loader2,
  Layers3,
  UsersRound,
} from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useAcademicYears, useSchoolClasses, useSections, useSubjects, useTeachers } from "../hooks/useAcademicData";

const academicAreas = [
  { label: "Academic Years", description: "Set school sessions and the current year.", icon: CalendarDays, path: "/academic/years", color: "border-t-amber-400" },
  { label: "Classes & Sections", description: "Manage classes and their sections together.", icon: GraduationCap, path: "/academic/classes", color: "border-t-emerald-500" },
  { label: "Subjects", description: "Maintain the school subject catalog.", icon: BookOpen, path: "/academic/subjects", color: "border-t-rose-400" },
  { label: "Class Subjects", description: "Choose subjects included in each class.", icon: Layers3, path: "/academic/class-subjects", color: "border-t-violet-400" },
  { label: "Teachers", description: "Manage teacher records and qualifications.", icon: UsersRound, path: "/academic/teachers", color: "border-t-cyan-500" },
  { label: "Teacher Assignments", description: "Connect teachers with their subjects.", icon: Link2, path: "/academic/teacher-assignments", color: "border-t-orange-400" },
];

export function AcademicManagementPage() {
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
            <h1 className="mt-2 font-display text-2xl font-semibold sm:text-3xl">Academic setup</h1>
            <p className="mt-2 max-w-2xl text-sm text-primary-foreground/80">Manage classes, curriculum, academic years, and teacher assignments from one place.</p>
          </div>
          <Link to="/academic/classes" className="inline-flex items-center justify-center rounded-md bg-background px-4 py-2 text-sm font-medium text-primary transition hover:bg-background/90">
            <GraduationCap aria-hidden="true" className="mr-2 h-4 w-4" />
            Manage classes
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

          <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
            {academicAreas.map((area) => {
              const Icon = area.icon;
              return (
                <Link key={area.path} to={area.path} className="group rounded-lg focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring">
                  <Card className={`h-full border-t-4 transition duration-200 group-hover:-translate-y-0.5 group-hover:shadow-md ${area.color}`}>
                    <CardContent className="flex h-full items-start gap-4 p-5">
                      <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary transition group-hover:bg-primary group-hover:text-primary-foreground">
                        <Icon aria-hidden="true" className="h-5 w-5" />
                      </span>
                      <span>
                        <span className="block font-semibold text-foreground">{area.label}</span>
                        <span className="mt-1 block text-sm text-muted-foreground">{area.description}</span>
                      </span>
                    </CardContent>
                  </Card>
                </Link>
              );
            })}
          </div>
        </>
      )}
    </div>
  );
}
