import { Link } from "react-router-dom";
import type { LucideIcon } from "lucide-react";
import {
  ArrowRight,
  BookOpen,
  CalendarDays,
  CheckCircle2,
  ClipboardPenLine,
  Layers,
  Loader2,
  Medal,
  Timer,
  Trophy,
} from "lucide-react";
import { Badge } from "@/components/ui/badge";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { PermissionGuard } from "@/features/auth/components/PermissionGuard";
import { Permission } from "@/lib/permissions";
import { useExamDashboard } from "../hooks/useExamData";
import { examStatusLabel } from "../types/exam.types";

const LINKS = [
  {
    to: "/exams/list",
    title: "Exams",
    description: "Create exams, then publish, complete, or cancel them.",
    icon: BookOpen,
    color: "border-t-emerald-500",
    iconColor: "text-emerald-700 bg-emerald-50 group-hover:bg-emerald-600 group-hover:text-white dark:bg-emerald-950/60 dark:text-emerald-300",
    permission: Permission.ExamView,
  },
  {
    to: "/exams/types",
    title: "Exam types",
    description: "Categories such as Term, Half-Yearly, and Final.",
    icon: Layers,
    color: "border-t-sky-500",
    iconColor: "text-sky-700 bg-sky-50 group-hover:bg-sky-600 group-hover:text-white dark:bg-sky-950/60 dark:text-sky-300",
    permission: Permission.ExamTypeView,
  },
  {
    to: "/exams/grades",
    title: "Grade setup",
    description: "GPA bands used later when results are calculated.",
    icon: Medal,
    color: "border-t-violet-400",
    iconColor: "text-violet-700 bg-violet-50 group-hover:bg-violet-600 group-hover:text-white dark:bg-violet-950/60 dark:text-violet-300",
    permission: Permission.GradeSetupView,
  },
  {
    to: "/exams/weights",
    title: "Weight setup",
    description: "Configure the contribution of each exam in final result calculation.",
    icon: Trophy,
    color: "border-t-amber-400",
    iconColor: "text-amber-800 bg-amber-50 group-hover:bg-amber-500 group-hover:text-white dark:bg-amber-950/50 dark:text-amber-300",
    permission: Permission.WeightSetupView,
  },
  {
    to: "/exams/calendar",
    title: "Exam calendar",
    description: "Subject-wise dates across a selected range.",
    icon: CalendarDays,
    color: "border-t-cyan-500",
    iconColor: "text-cyan-800 bg-cyan-50 group-hover:bg-cyan-600 group-hover:text-white dark:bg-cyan-950/60 dark:text-cyan-300",
    permission: Permission.ExamView,
  },
  {
    to: "/results/marks",
    title: "Marks entry",
    description: "Enter, submit, and lock marks for each scheduled paper.",
    icon: ClipboardPenLine,
    color: "border-t-orange-400",
    iconColor: "text-orange-800 bg-orange-50 group-hover:bg-orange-500 group-hover:text-white dark:bg-orange-950/50 dark:text-orange-300",
    permission: Permission.MarksEntryView,
  },
  {
    to: "/results/exams",
    title: "Exam results",
    description: "Calculate and publish student results after marks are submitted.",
    icon: Trophy,
    color: "border-t-rose-400",
    iconColor: "text-rose-700 bg-rose-50 group-hover:bg-rose-600 group-hover:text-white dark:bg-rose-950/60 dark:text-rose-300",
    permission: Permission.ResultView,
  },
  {
    to: "/results/final",
    title: "Final result",
    description: "Generate year-end weighted results based on the active exam weight setup.",
    icon: Medal,
    color: "border-t-indigo-400",
    iconColor: "text-indigo-700 bg-indigo-50 group-hover:bg-indigo-600 group-hover:text-white dark:bg-indigo-950/60 dark:text-indigo-300",
    permission: Permission.FinalResultView,
  },
  {
    to: "/results/audit",
    title: "Result audit trail",
    description: "Review the history of calculations, publishes, unlocks, and mark changes.",
    icon: CalendarDays,
    color: "border-t-fuchsia-400",
    iconColor: "text-fuchsia-700 bg-fuchsia-50 group-hover:bg-fuchsia-600 group-hover:text-white dark:bg-fuchsia-950/60 dark:text-fuchsia-300",
    permission: Permission.ResultAuditView,
  },
] as const;

export function ExamHubPage() {
  const { data, isPending, isError, error, refetch, isFetching } = useExamDashboard();

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <Card className="overflow-hidden border-primary/20 bg-gradient-to-br from-primary to-primary/90 text-primary-foreground">
        <CardContent className="flex flex-col gap-4 p-6 sm:flex-row sm:items-center sm:justify-between sm:p-8">
          <div>
            <p className="text-xs font-semibold uppercase tracking-[0.2em] text-primary-foreground/70">
              Academic year operations
            </p>
            <h1 className="mt-2 font-display text-2xl font-semibold sm:text-3xl">
              Examination management
            </h1>
            <p className="mt-2 max-w-2xl text-sm leading-6 text-primary-foreground/80">
              Set up exams, schedules, grading and result workflows for your
              school.
            </p>
          </div>
          <div className="flex h-14 w-14 shrink-0 items-center justify-center self-start rounded-2xl border border-white/15 bg-white/10 sm:self-center">
            <BookOpen aria-hidden="true" className="h-7 w-7" />
          </div>
        </CardContent>
      </Card>

      {isPending ? (
        <div className="flex min-h-32 items-center justify-center rounded-xl border border-dashed bg-card/70 p-8 text-sm text-muted-foreground">
          <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
          Loading exam overview…
        </div>
      ) : isError || !data ? (
        <Card className="border-destructive/20 bg-destructive/5">
          <CardContent className="flex flex-col gap-3 p-5 sm:flex-row sm:items-center sm:justify-between">
            <div className="text-sm">
              <p className="font-medium text-destructive">Exam overview is unavailable.</p>
              <p className="mt-1 text-muted-foreground">
                {error instanceof Error
                  ? error.message
                  : "You can still open the exam sections below."}
              </p>
            </div>
            <button
              type="button"
              onClick={() => void refetch()}
              disabled={isFetching}
              className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent disabled:opacity-60"
            >
              {isFetching ? "Retrying…" : "Try again"}
            </button>
          </CardContent>
        </Card>
      ) : (
        <section aria-label="Exam overview" className="grid gap-3 grid-cols-2 lg:grid-cols-3 xl:grid-cols-5">
          <Stat label="Total exams" value={data.totalExams} color="border-t-emerald-500" icon={BookOpen} />
          <Stat label="Draft" value={data.draftExams} color="border-t-sky-500" icon={Layers} />
          <Stat label="Published" value={data.publishedExams} color="border-t-amber-400" icon={CheckCircle2} />
          <Stat label="Completed" value={data.completedExams} color="border-t-violet-400" icon={Trophy} />
          <Stat label="Upcoming" value={data.upcomingExamsCount} color="border-t-rose-400" icon={Timer} />
        </section>
      )}

      <section aria-label="Exam and result sections" className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
        {LINKS.map((item) => (
          <PermissionGuard key={item.to} permission={item.permission}>
            <Link to={item.to} className="group block h-full rounded-lg focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring">
              <Card className={`h-full min-h-44 border-t-4 transition duration-200 group-hover:-translate-y-0.5 group-hover:shadow-md ${item.color}`}>
                <CardContent className="flex h-full flex-col p-5">
                  <span className={`mb-4 flex h-11 w-11 items-center justify-center rounded-xl transition-colors ${item.iconColor}`}>
                    <item.icon aria-hidden="true" className="h-5 w-5" />
                  </span>
                  <CardTitle className="text-base">{item.title}</CardTitle>
                  <CardDescription className="mt-2 flex-1 leading-5">{item.description}</CardDescription>
                  <span className="mt-5 inline-flex items-center gap-2 text-sm font-semibold text-primary">
                    Open section
                    <ArrowRight aria-hidden="true" className="h-4 w-4 transition-transform group-hover:translate-x-1" />
                  </span>
                </CardContent>
              </Card>
            </Link>
          </PermissionGuard>
        ))}
      </section>

      {data?.upcomingExams?.length ? (
        <Card>
          <CardHeader className="pb-4">
            <CardTitle className="flex items-center gap-2">
              <CalendarDays aria-hidden="true" className="h-5 w-5 text-primary" />
              Upcoming exams
            </CardTitle>
            <CardDescription>Exams with the next scheduled papers.</CardDescription>
          </CardHeader>
          <CardContent className="grid gap-3 md:grid-cols-2 xl:grid-cols-3">
            {data.upcomingExams.map((exam, index) => (
              <Link
                key={exam.examId}
                to={`/exams/${exam.examId}`}
                className={`group flex items-center gap-4 rounded-lg border border-l-4 p-4 transition hover:-translate-y-0.5 hover:bg-muted/30 hover:shadow-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring ${
                  ["border-l-emerald-500", "border-l-sky-500", "border-l-amber-400"][index % 3]
                }`}
              >
                <div className="flex h-12 w-12 shrink-0 flex-col items-center justify-center rounded-lg bg-primary/10 text-primary">
                  <span className="text-lg font-bold leading-none">{exam.daysRemaining}</span>
                  <span className="mt-1 text-[9px] font-semibold uppercase tracking-wide">days</span>
                </div>
                <div className="min-w-0 flex-1">
                  <p className="truncate font-semibold">{exam.examName}</p>
                  <p className="mt-1 text-sm text-muted-foreground">{exam.examTypeName}</p>
                  <p className="mt-2 text-xs text-muted-foreground">
                    {new Date(exam.nextExamDate).toLocaleDateString("en-BD", {
                      day: "numeric",
                      month: "short",
                      year: "numeric",
                    })}
                    {" · "}
                    {exam.totalSchedules} papers
                  </p>
                </div>
                <ArrowRight aria-hidden="true" className="h-4 w-4 shrink-0 text-primary transition-transform group-hover:translate-x-1" />
              </Link>
            ))}
          </CardContent>
        </Card>
      ) : null}

      {data?.recentExams?.length ? (
        <Card>
          <CardHeader className="pb-4">
            <CardTitle className="flex items-center gap-2">
              <Trophy aria-hidden="true" className="h-5 w-5 text-amber-500" />
              Recent exams
            </CardTitle>
            <CardDescription>Latest exams across academic years.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-2">
            {data.recentExams.map((exam, index) => (
              <Link
                key={exam.id}
                to={`/exams/${exam.id}`}
                className={`flex flex-col gap-3 rounded-lg border border-l-4 p-4 transition hover:bg-muted/30 sm:flex-row sm:items-center sm:justify-between ${
                  ["border-l-violet-400", "border-l-cyan-500", "border-l-rose-400"][index % 3]
                }`}
              >
                <div className="min-w-0">
                  <p className="truncate font-semibold">{exam.name}</p>
                  <p className="mt-1 text-sm text-muted-foreground">
                    {exam.examTypeName} · {exam.academicYearName}
                  </p>
                </div>
                <Badge
                  variant="outline"
                  className={
                    exam.status === 1
                      ? "w-fit border-sky-200 bg-sky-50 text-sky-800 dark:border-sky-900 dark:bg-sky-950/50 dark:text-sky-200"
                      : exam.status === 2
                        ? "w-fit border-amber-200 bg-amber-50 text-amber-800 dark:border-amber-900 dark:bg-amber-950/50 dark:text-amber-200"
                        : exam.status === 3
                          ? "w-fit border-emerald-200 bg-emerald-50 text-emerald-800 dark:border-emerald-900 dark:bg-emerald-950/50 dark:text-emerald-200"
                          : "w-fit border-rose-200 bg-rose-50 text-rose-800 dark:border-rose-900 dark:bg-rose-950/50 dark:text-rose-200"
                  }
                >
                  {examStatusLabel(exam.status)}
                </Badge>
              </Link>
            ))}
          </CardContent>
        </Card>
      ) : null}
    </div>
  );
}

function Stat({
  label,
  value,
  color,
  icon: Icon,
}: {
  label: string;
  value: number;
  color: string;
  icon: LucideIcon;
}) {
  return (
    <Card className={`border-t-4 ${color}`}>
      <CardContent className="flex items-center justify-between gap-3 p-4">
        <div>
          <p className="text-xs font-medium text-muted-foreground">{label}</p>
          <p className="mt-1 text-2xl font-semibold tabular-nums">{value}</p>
        </div>
        <span className="flex h-10 w-10 items-center justify-center rounded-lg bg-primary/10 text-primary">
          <Icon aria-hidden="true" className="h-5 w-5" />
        </span>
      </CardContent>
    </Card>
  );
}
