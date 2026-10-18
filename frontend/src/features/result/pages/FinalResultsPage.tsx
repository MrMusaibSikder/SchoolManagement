import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import {
  Award,
  BadgeCheck,
  Loader2,
  RefreshCw,
  Send,
  Unlock,
  Users,
} from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useAcademicYears } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { getResultErrorMessage } from "../api/result.api";
import { useFinalResultLifecycle, useFinalResults } from "../hooks/useResultData";

export function FinalResultsPage() {
  const { hasPermission } = usePermissions();
  const { data: academicYears = [] } = useAcademicYears();
  const [selectedYearId, setSelectedYearId] = useState<string>(academicYears[0]?.id ? String(academicYears[0].id) : "");
  const [classFilter, setClassFilter] = useState<string>("");

  const finalResultsQuery = useFinalResults(selectedYearId ? Number(selectedYearId) : null, classFilter ? Number(classFilter) : undefined);
  const lifecycle = useFinalResultLifecycle();

  const classNames = useMemo(() => {
    const set = new Set<string>();
    finalResultsQuery.data?.forEach((item) => {
      if (item.className) set.add(item.className);
    });
    return Array.from(set).sort();
  }, [finalResultsQuery.data]);

  const summary = useMemo(() => {
    const results = finalResultsQuery.data ?? [];
    const passedCount = results.filter((result) => result.isPassed).length;
    const publishedCount = results.filter((result) => result.isPublished).length;
    const averageGpa = results.length
      ? results.reduce((total, result) => total + result.finalGpa, 0) / results.length
      : 0;

    return {
      studentCount: results.length,
      passRate: results.length ? Math.round((passedCount / results.length) * 100) : 0,
      averageGpa: averageGpa.toFixed(2),
      publishedCount,
    };
  }, [finalResultsQuery.data]);

  async function run(action: "calculate" | "publish" | "unpublish") {
    if (!selectedYearId) return;

    try {
      await lifecycle[action].mutateAsync(Number(selectedYearId));
      toast.success(
        action === "calculate"
          ? "Final results calculated."
          : action === "publish"
            ? "Final results published."
            : "Final results unpublished."
      );
    } catch (error) {
      toast.error(getResultErrorMessage(error));
    }
  }

  if (!hasPermission(Permission.FinalResultView)) {
    return (
      <Card>
        <CardContent className="p-6 text-sm text-destructive">
          You do not have permission to view final results.
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <section className="relative isolate overflow-hidden rounded-2xl bg-gradient-to-r from-emerald-700 via-teal-700 to-cyan-800 px-6 py-7 text-white shadow-lg shadow-emerald-950/10 sm:px-8">
        <div aria-hidden="true" className="absolute -right-10 -top-20 -z-10 h-64 w-64 rounded-full bg-white/10 blur-2xl" />
        <div aria-hidden="true" className="absolute -bottom-28 right-1/3 -z-10 h-52 w-52 rounded-full bg-cyan-300/20 blur-2xl" />
        <div className="flex flex-col gap-5 sm:flex-row sm:items-center sm:justify-between">
          <div className="flex items-start gap-4">
            <span className="rounded-xl bg-white/15 p-3 ring-1 ring-white/20">
              <Award aria-hidden="true" className="h-7 w-7" />
            </span>
            <div>
              <p className="mb-1 text-xs font-semibold uppercase tracking-[0.18em] text-emerald-100">
                Academic performance
              </p>
              <h1 className="font-display text-2xl font-semibold tracking-tight sm:text-3xl">Final result</h1>
              <p className="mt-2 max-w-2xl text-sm text-emerald-50/90">
                Year-end weighted summary based on the active exam weight setup.
              </p>
            </div>
          </div>
          <Link
            to="/exams"
            className="inline-flex w-fit items-center rounded-lg border border-white/30 bg-white/10 px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-white/20 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white"
          >
            Back to exams
          </Link>
        </div>
      </section>

      <Card className="border-emerald-100/80 shadow-sm dark:border-emerald-950">
        <CardHeader className="pb-4">
          <div className="flex items-center gap-3">
            <span className="rounded-lg bg-emerald-100 p-2 text-emerald-700 dark:bg-emerald-950 dark:text-emerald-300">
              <Users aria-hidden="true" className="h-5 w-5" />
            </span>
            <div>
              <CardTitle>Result filters</CardTitle>
              <CardDescription className="mt-1">Choose an academic year and optionally narrow results by class.</CardDescription>
            </div>
          </div>
        </CardHeader>
        <CardContent className="flex flex-col gap-4 md:flex-row md:items-end">
          <div className="grid w-full gap-1.5 md:max-w-xs">
            <label htmlFor="academic-year" className="text-sm font-medium">Academic year</label>
            <select
              id="academic-year"
              value={selectedYearId}
              onChange={(event) => setSelectedYearId(event.target.value)}
              className="h-10 rounded-lg border border-input bg-background px-3 text-sm shadow-sm outline-none transition focus-visible:ring-2 focus-visible:ring-emerald-600/40"
            >
              <option value="">Select academic year</option>
              {academicYears.map((year) => (
                <option key={year.id} value={year.id}>
                  {year.name}
                </option>
              ))}
            </select>
          </div>

          <div className="grid w-full gap-1.5 md:max-w-xs">
            <label htmlFor="result-class" className="text-sm font-medium">Class</label>
            <select
              id="result-class"
              value={classFilter}
              onChange={(event) => setClassFilter(event.target.value)}
              className="h-10 rounded-lg border border-input bg-background px-3 text-sm shadow-sm outline-none transition focus-visible:ring-2 focus-visible:ring-teal-600/40"
            >
              <option value="">All classes</option>
              {classNames.map((className) => (
                <option key={className} value={className}>
                  {className}
                </option>
              ))}
            </select>
          </div>

          <div className="flex flex-wrap gap-2 md:ml-auto">
            {hasPermission(Permission.FinalResultCalculate) ? (
              <Button
                type="button"
                className="bg-emerald-700 text-white shadow-sm hover:bg-emerald-800"
                disabled={lifecycle.calculate.isPending || !selectedYearId}
                onClick={() => void run("calculate")}
              >
                <RefreshCw className="mr-2 h-4 w-4" />
                Calculate
              </Button>
            ) : null}

            {hasPermission(Permission.FinalResultPublish) ? (
              <Button
                type="button"
                variant="outline"
                className="border-sky-200 text-sky-700 hover:bg-sky-50 dark:border-sky-900 dark:text-sky-300 dark:hover:bg-sky-950"
                disabled={lifecycle.publish.isPending || !selectedYearId}
                onClick={() => void run("publish")}
              >
                <Send className="mr-2 h-4 w-4" />
                Publish
              </Button>
            ) : null}

            {hasPermission(Permission.FinalResultUnlock) ? (
              <Button
                type="button"
                variant="outline"
                className="border-amber-200 text-amber-800 hover:bg-amber-50 dark:border-amber-900 dark:text-amber-300 dark:hover:bg-amber-950"
                disabled={lifecycle.unpublish.isPending || !selectedYearId}
                onClick={() => void run("unpublish")}
              >
                <Unlock className="mr-2 h-4 w-4" />
                Unpublish
              </Button>
            ) : null}
          </div>
        </CardContent>
      </Card>

      {finalResultsQuery.data && finalResultsQuery.data.length > 0 ? (
        <section aria-label="Final result overview" className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
          <SummaryCard
            label="Students"
            value={summary.studentCount}
            icon={Users}
            color="blue"
          />
          <SummaryCard
            label="Pass rate"
            value={`${summary.passRate}%`}
            icon={BadgeCheck}
            color="emerald"
          />
          <SummaryCard
            label="Average GPA"
            value={summary.averageGpa}
            icon={Award}
            color="amber"
          />
          <SummaryCard
            label="Published results"
            value={`${summary.publishedCount} / ${summary.studentCount}`}
            icon={Send}
            color="violet"
          />
        </section>
      ) : null}

      {finalResultsQuery.isPending ? (
        <Card className="border-sky-100 dark:border-sky-950">
          <CardContent className="flex items-center gap-3 p-5 text-sm text-sky-800 dark:text-sky-200">
            <Loader2 className="h-5 w-5 animate-spin text-sky-600" />
            Loading final results…
          </CardContent>
        </Card>
      ) : null}

      {finalResultsQuery.isError ? (
        <Card className="border-rose-200 bg-rose-50 dark:border-rose-950 dark:bg-rose-950/30">
          <CardContent className="p-5 text-sm font-medium text-rose-800 dark:text-rose-200">
            Unable to load final results.
          </CardContent>
        </Card>
      ) : null}

      {!finalResultsQuery.isPending && !finalResultsQuery.isError && (!finalResultsQuery.data || finalResultsQuery.data.length === 0) ? (
        <Card className="border-dashed border-amber-200 bg-amber-50/50 dark:border-amber-950 dark:bg-amber-950/20">
          <CardContent className="flex flex-col items-center px-6 py-10 text-center">
            <span className="mb-4 rounded-2xl bg-amber-100 p-4 text-amber-700 dark:bg-amber-950 dark:text-amber-300">
              <Award aria-hidden="true" className="h-7 w-7" />
            </span>
            <p className="font-semibold text-foreground">No final results yet</p>
            <p className="mt-1 max-w-md text-sm text-muted-foreground">
              No final results are available for the selected academic year.
            </p>
          </CardContent>
        </Card>
      ) : null}

      {finalResultsQuery.data && finalResultsQuery.data.length > 0 ? (
        <Card className="overflow-hidden border-slate-200 shadow-sm dark:border-slate-800">
          <CardHeader className="border-b bg-gradient-to-r from-slate-50 via-white to-emerald-50/70 dark:from-slate-950 dark:via-card dark:to-emerald-950/30">
            <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <CardTitle>Final result list</CardTitle>
                <CardDescription className="mt-1">
                  {summary.studentCount} students · {summary.publishedCount} results published
                </CardDescription>
              </div>
              <span className="w-fit rounded-full bg-emerald-100 px-3 py-1 text-xs font-semibold text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200">
                {academicYears.find((year) => String(year.id) === selectedYearId)?.name ?? "Academic year"}
              </span>
            </div>
          </CardHeader>
          <div className="overflow-x-auto">
            <table aria-label="Final student results" className="w-full min-w-[900px] text-sm">
              <thead className="bg-slate-50/80 text-slate-600 dark:bg-slate-900/70 dark:text-slate-300">
                <tr className="border-b text-left">
                  <th scope="col" className="px-4 py-3 font-semibold">Student</th>
                  <th scope="col" className="px-4 py-3 font-semibold">Roll</th>
                  <th scope="col" className="px-4 py-3 font-semibold">Class</th>
                  <th scope="col" className="px-4 py-3 font-semibold">Section</th>
                  <th scope="col" className="px-4 py-3 font-semibold">Final marks</th>
                  <th scope="col" className="px-4 py-3 font-semibold">GPA</th>
                  <th scope="col" className="px-4 py-3 font-semibold">Grade</th>
                  <th scope="col" className="px-4 py-3 font-semibold">Status</th>
                  <th scope="col" className="px-4 py-3 font-semibold">Published</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-border">
                {finalResultsQuery.data.map((result) => (
                  <tr key={result.id} className="transition-colors hover:bg-emerald-50/50 dark:hover:bg-emerald-950/20">
                    <td className="px-4 py-3 font-medium text-foreground">{result.studentName}</td>
                    <td className="px-4 py-3 tabular-nums text-muted-foreground">{result.rollNo}</td>
                    <td className="px-4 py-3">{result.className}</td>
                    <td className="px-4 py-3">{result.sectionName}</td>
                    <td className="px-4 py-3 font-semibold tabular-nums text-slate-700 dark:text-slate-200">{result.finalMarks}</td>
                    <td className="px-4 py-3 font-semibold tabular-nums text-indigo-700 dark:text-indigo-300">{result.finalGpa}</td>
                    <td className="px-4 py-3">
                      <span className="inline-flex min-w-10 justify-center rounded-md bg-violet-100 px-2 py-1 text-xs font-bold text-violet-800 dark:bg-violet-950 dark:text-violet-200">
                        {result.finalGrade}
                      </span>
                    </td>
                    <td className="px-4 py-3">
                      <span className={`inline-flex items-center rounded-full px-2.5 py-1 text-xs font-semibold ${
                        result.isPassed
                          ? "bg-emerald-100 text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200"
                          : "bg-rose-100 text-rose-800 dark:bg-rose-950 dark:text-rose-200"
                      }`}>
                        {result.isPassed ? "Passed" : "Failed"}
                      </span>
                    </td>
                    <td className="px-4 py-3">
                      <span className={`inline-flex rounded-full px-2.5 py-1 text-xs font-medium ${
                        result.isPublished
                          ? "bg-sky-100 text-sky-800 dark:bg-sky-950 dark:text-sky-200"
                          : "bg-slate-100 text-slate-600 dark:bg-slate-800 dark:text-slate-300"
                      }`}>
                        {result.isPublished ? "Published" : "Pending"}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </Card>
      ) : null}
    </div>
  );
}

function SummaryCard({
  label,
  value,
  icon: Icon,
  color,
}: {
  label: string;
  value: number | string;
  icon: typeof Users;
  color: "blue" | "emerald" | "amber" | "violet";
}) {
  const colorClasses = {
    blue: "bg-blue-100 text-blue-700 dark:bg-blue-950 dark:text-blue-300",
    emerald: "bg-emerald-100 text-emerald-700 dark:bg-emerald-950 dark:text-emerald-300",
    amber: "bg-amber-100 text-amber-800 dark:bg-amber-950 dark:text-amber-300",
    violet: "bg-violet-100 text-violet-700 dark:bg-violet-950 dark:text-violet-300",
  };

  return (
    <Card className="border-slate-200/80 shadow-sm transition-shadow hover:shadow-md dark:border-slate-800">
      <CardContent className="flex items-center gap-4 p-4">
        <span className={`rounded-xl p-3 ${colorClasses[color]}`}>
          <Icon aria-hidden="true" className="h-5 w-5" />
        </span>
        <div className="min-w-0">
          <p className="text-sm text-muted-foreground">{label}</p>
          <p className="mt-0.5 truncate text-2xl font-semibold tracking-tight tabular-nums text-foreground">{value}</p>
        </div>
      </CardContent>
    </Card>
  );
}
