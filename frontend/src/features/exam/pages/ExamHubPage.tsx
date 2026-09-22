import { Link } from "react-router-dom";
import { BookOpen, CalendarDays, ClipboardPenLine, Layers, Loader2, Medal, Trophy } from "lucide-react";
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
    permission: Permission.ExamView,
  },
  {
    to: "/exams/types",
    title: "Exam types",
    description: "Categories such as Term, Half-Yearly, and Final.",
    icon: Layers,
    permission: Permission.ExamTypeView,
  },
  {
    to: "/exams/grades",
    title: "Grade setup",
    description: "GPA bands used later when results are calculated.",
    icon: Medal,
    permission: Permission.GradeSetupView,
  },
  {
    to: "/exams/weights",
    title: "Weight setup",
    description: "Configure the contribution of each exam in final result calculation.",
    icon: Trophy,
    permission: Permission.WeightSetupView,
  },
  {
    to: "/exams/calendar",
    title: "Exam calendar",
    description: "Subject-wise dates across a selected range.",
    icon: CalendarDays,
    permission: Permission.ExamView,
  },
  {
    to: "/results/marks",
    title: "Marks entry",
    description: "Enter, submit, and lock marks for each scheduled paper.",
    icon: ClipboardPenLine,
    permission: Permission.MarksEntryView,
  },
  {
    to: "/results/exams",
    title: "Exam results",
    description: "Calculate and publish student results after marks are submitted.",
    icon: Trophy,
    permission: Permission.ResultView,
  },
  {
    to: "/results/final",
    title: "Final result",
    description: "Generate year-end weighted results based on the active exam weight setup.",
    icon: Medal,
    permission: Permission.FinalResultView,
  },
] as const;

export function ExamHubPage() {
  const { data, isPending, isError } = useExamDashboard();

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div>
        <h1 className="font-display text-2xl font-semibold">Examinations</h1>
        <p className="text-sm text-muted-foreground">
          Draft → Published → Completed. Cancelled exams can be reopened to Draft. Marks stay in the Result module.
        </p>
      </div>

      {isPending ? (
        <div className="flex items-center text-sm text-muted-foreground">
          <Loader2 className="mr-2 h-4 w-4 animate-spin" /> Loading exam overview…
        </div>
      ) : isError || !data ? (
        <Card>
          <CardContent className="p-6 text-sm text-muted-foreground">
            Exam overview is unavailable. You can still open the lists below.
          </CardContent>
        </Card>
      ) : (
        <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-5">
          <Stat label="Total" value={data.totalExams} />
          <Stat label="Draft" value={data.draftExams} />
          <Stat label="Published" value={data.publishedExams} />
          <Stat label="Completed" value={data.completedExams} />
          <Stat label="Upcoming" value={data.upcomingExamsCount} />
        </div>
      )}

      <div className="grid gap-4 md:grid-cols-2">
        {LINKS.map((item) => (
          <PermissionGuard key={item.to} permission={item.permission}>
            <Link to={item.to} className="block h-full">
              <Card className="h-full transition hover:border-primary/40 hover:bg-muted/30">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2 text-lg">
                    <item.icon className="h-5 w-5 text-muted-foreground" />
                    {item.title}
                  </CardTitle>
                  <CardDescription>{item.description}</CardDescription>
                </CardHeader>
                <CardContent className="text-sm font-medium text-primary">Open</CardContent>
              </Card>
            </Link>
          </PermissionGuard>
        ))}
      </div>

      {data?.upcomingExams?.length ? (
        <Card>
          <CardHeader>
            <CardTitle>Upcoming exams</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            {data.upcomingExams.map((exam) => (
              <Link
                key={exam.examId}
                to={`/exams/${exam.examId}`}
                className="flex items-center justify-between rounded-lg border p-3 hover:bg-muted/40"
              >
                <div>
                  <p className="font-medium">{exam.examName}</p>
                  <p className="text-sm text-muted-foreground">{exam.examTypeName}</p>
                </div>
                <p className="text-sm text-muted-foreground">{exam.daysRemaining} days left</p>
              </Link>
            ))}
          </CardContent>
        </Card>
      ) : null}

      {data?.recentExams?.length ? (
        <Card>
          <CardHeader>
            <CardTitle>Recent exams</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            {data.recentExams.map((exam) => (
              <Link
                key={exam.id}
                to={`/exams/${exam.id}`}
                className="flex items-center justify-between rounded-lg border p-3 hover:bg-muted/40"
              >
                <div>
                  <p className="font-medium">{exam.name}</p>
                  <p className="text-sm text-muted-foreground">
                    {exam.examTypeName} • {exam.academicYearName}
                  </p>
                </div>
                <span className="text-xs font-medium">{examStatusLabel(exam.status)}</span>
              </Link>
            ))}
          </CardContent>
        </Card>
      ) : null}
    </div>
  );
}

function Stat({ label, value }: { label: string; value: number }) {
  return (
    <div className="rounded-lg border bg-muted/30 p-3">
      <p className="text-xs text-muted-foreground">{label}</p>
      <p className="mt-1 text-xl font-semibold tabular-nums">{value}</p>
    </div>
  );
}
