import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, RefreshCw, Send, Unlock } from "lucide-react";
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
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Final result</h1>
          <p className="text-sm text-muted-foreground">
            Year-end weighted summary based on the active exam weight setup.
          </p>
        </div>
        <Link to="/exams" className="inline-flex items-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent">
          Back to exams
        </Link>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Academic year</CardTitle>
          <CardDescription>Select the year and optional class filter.</CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-3 md:flex-row">
          <select
            value={selectedYearId}
            onChange={(event) => setSelectedYearId(event.target.value)}
            className="rounded-md border bg-background px-3 py-2 md:max-w-xs"
          >
            <option value="">Select academic year</option>
            {academicYears.map((year) => (
              <option key={year.id} value={year.id}>
                {year.name}
              </option>
            ))}
          </select>

          <select
            value={classFilter}
            onChange={(event) => setClassFilter(event.target.value)}
            className="rounded-md border bg-background px-3 py-2 md:max-w-xs"
          >
            <option value="">All classes</option>
            {classNames.map((className) => (
              <option key={className} value={className}>
                {className}
              </option>
            ))}
          </select>

          <div className="flex flex-wrap gap-2 md:ml-auto">
            {hasPermission(Permission.FinalResultCalculate) ? (
              <Button type="button" disabled={lifecycle.calculate.isPending || !selectedYearId} onClick={() => void run("calculate")}>
                <RefreshCw className="mr-2 h-4 w-4" />
                Calculate
              </Button>
            ) : null}

            {hasPermission(Permission.FinalResultPublish) ? (
              <Button
                type="button"
                variant="outline"
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

      {finalResultsQuery.isPending ? (
        <p className="text-sm text-muted-foreground">
          <Loader2 className="mr-2 inline h-4 w-4 animate-spin" />
          Loading final results…
        </p>
      ) : null}

      {finalResultsQuery.isError ? (
        <p className="text-sm text-destructive">Unable to load final results.</p>
      ) : null}

      {!finalResultsQuery.isPending && !finalResultsQuery.isError && (!finalResultsQuery.data || finalResultsQuery.data.length === 0) ? (
        <Card>
          <CardContent className="p-6 text-sm text-muted-foreground">
            No final results are available for the selected academic year.
          </CardContent>
        </Card>
      ) : null}

      {finalResultsQuery.data?.length ? (
        <Card>
          <CardHeader>
            <CardTitle>Result table</CardTitle>
          </CardHeader>
          <CardContent className="overflow-x-auto">
            <table className="w-full min-w-[900px] text-sm">
              <thead>
                <tr className="border-b text-left">
                  <th className="p-2">Student</th>
                  <th className="p-2">Roll</th>
                  <th className="p-2">Class</th>
                  <th className="p-2">Section</th>
                  <th className="p-2">Final marks</th>
                  <th className="p-2">GPA</th>
                  <th className="p-2">Grade</th>
                  <th className="p-2">Status</th>
                  <th className="p-2">Published</th>
                </tr>
              </thead>
              <tbody>
                {finalResultsQuery.data.map((result) => (
                  <tr key={result.id} className="border-b">
                    <td className="p-2 font-medium">{result.studentName}</td>
                    <td className="p-2">{result.rollNo}</td>
                    <td className="p-2">{result.className}</td>
                    <td className="p-2">{result.sectionName}</td>
                    <td className="p-2 tabular-nums">{result.finalMarks}</td>
                    <td className="p-2 tabular-nums">{result.finalGpa}</td>
                    <td className="p-2">{result.finalGrade}</td>
                    <td className="p-2">
                      <span className="rounded-full border px-2 py-1 text-xs font-medium">
                        {result.isPassed ? "Passed" : "Failed"}
                      </span>
                    </td>
                    <td className="p-2">{result.isPublished ? "Yes" : "No"}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </CardContent>
        </Card>
      ) : null}
    </div>
  );
}
