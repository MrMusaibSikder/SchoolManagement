import { Link, useParams } from "react-router-dom";
import { Loader2, RefreshCw, Send, Unlock } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useState } from "react";
import { useExams } from "@/features/exam/hooks/useExamData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { getResultErrorMessage } from "../api/result.api";
import { useExamResultDashboard, useExamResultLifecycle, useExamResults } from "../hooks/useResultData";

export function ExamResultsPage() {
  const { id } = useParams();
  const examId = Number(id);
  const { hasPermission } = usePermissions();
  const { data: exams = [] } = useExams();
  const [selectedExamId, setSelectedExamId] = useState(Number.isNaN(examId) ? "" : String(examId));
  const activeExamId = selectedExamId ? Number(selectedExamId) : null;
  const { data: dashboard, isPending: dashboardPending } = useExamResultDashboard(activeExamId);
  const { data: results = [], isPending: resultsPending, isError } = useExamResults(activeExamId);
  const lifecycle = useExamResultLifecycle();

  async function run(action: "calculate" | "publish" | "unpublish") {
    try {
      if (!activeExamId) return;
      await lifecycle[action].mutateAsync(activeExamId);
      toast.success(action === "calculate" ? "Results calculated." : action === "publish" ? "Results published." : "Results unpublished.");
    } catch (error) {
      toast.error(getResultErrorMessage(error));
    }
  }

  if (!hasPermission(Permission.ResultView)) {
    return <Card><CardContent className="p-6 text-sm text-destructive">You do not have permission to view results.</CardContent></Card>;
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div><h1 className="font-display text-2xl font-semibold">Exam results</h1><p className="text-sm text-muted-foreground">Calculated result summary and publication controls.</p></div>
        <Link to="/exams/list" className="inline-flex items-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent">Back to exams</Link>
      </div>
      <Card><CardContent className="p-4"><select value={selectedExamId} onChange={(event) => setSelectedExamId(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2 md:max-w-md"><option value="">Select exam</option>{exams.map((exam) => <option key={exam.id} value={exam.id}>{exam.name}</option>)}</select></CardContent></Card>
      {dashboardPending ? <p className="text-sm text-muted-foreground"><Loader2 className="mr-2 inline h-4 w-4 animate-spin" />Loading result dashboard…</p> : null}
      {dashboard ? <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4"><Stat label="Students" value={dashboard.totalStudents} /><Stat label="Appeared" value={dashboard.appearedStudents} /><Stat label="Completion" value={`${dashboard.completionPercentage}%`} /><Stat label="Published" value={dashboard.publishedResultCount} /></div> : null}
      <Card><CardHeader><div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between"><div><CardTitle>{dashboard?.examName ?? "Result list"}</CardTitle><CardDescription>{dashboard?.isResultPublished ? "Published" : "Not published"} · {dashboard?.pendingResultCount ?? 0} pending</CardDescription></div><div className="flex flex-wrap gap-2">{hasPermission(Permission.ResultCalculate) ? <Button disabled={lifecycle.calculate.isPending} onClick={() => void run("calculate")}><RefreshCw className="mr-2 h-4 w-4" />Calculate</Button> : null}{hasPermission(Permission.ResultPublish) && !dashboard?.isResultPublished ? <Button variant="outline" disabled={lifecycle.publish.isPending} onClick={() => void run("publish")}><Send className="mr-2 h-4 w-4" />Publish</Button> : null}{hasPermission(Permission.ResultUnlock) && dashboard?.isResultPublished ? <Button variant="outline" disabled={lifecycle.unpublish.isPending} onClick={() => void run("unpublish")}><Unlock className="mr-2 h-4 w-4" />Unpublish</Button> : null}</div></div></CardHeader><CardContent>{resultsPending ? <p className="text-sm text-muted-foreground">Loading results…</p> : null}{isError ? <p className="text-sm text-destructive">Unable to load exam results.</p> : null}{!resultsPending && results.length === 0 ? <p className="text-sm text-muted-foreground">No calculated results yet. Calculate the exam after marks are submitted.</p> : null}{results.length > 0 ? <div className="overflow-x-auto"><table className="w-full min-w-[800px] text-sm"><thead><tr className="border-b text-left"><th className="p-2">Student</th><th className="p-2">Class</th><th className="p-2">Marks</th><th className="p-2">%</th><th className="p-2">GPA</th><th className="p-2">Grade</th><th className="p-2">Status</th></tr></thead><tbody>{results.map((result) => <tr key={result.id} className="border-b"><td className="p-2"><p className="font-medium">{result.studentName}</p><p className="text-xs text-muted-foreground">Roll {result.rollNo}</p></td><td className="p-2">{result.className} · {result.sectionName}</td><td className="p-2">{result.totalMarks} / {result.totalFullMarks}</td><td className="p-2">{result.percentage.toFixed(2)}</td><td className="p-2">{result.gpa.toFixed(2)}</td><td className="p-2 font-medium">{result.grade}</td><td className="p-2">{result.isPassed ? "Passed" : "Failed"}</td></tr>)}</tbody></table></div> : null}</CardContent></Card>
      {dashboard?.subjectStatistics?.length ? <Card><CardHeader><CardTitle>Subject statistics</CardTitle></CardHeader><CardContent className="grid gap-3 md:grid-cols-2 xl:grid-cols-3">{dashboard.subjectStatistics.map((item) => <div key={item.subjectId} className="rounded-lg border p-3"><p className="font-medium">{item.subjectName}</p><p className="text-sm text-muted-foreground">Average {item.averageMarks} · Pass rate {item.passRate}%</p><p className="text-xs text-muted-foreground">Highest {item.highestMarks} · Lowest {item.lowestMarks}</p></div>)}</CardContent></Card> : null}
    </div>
  );
}

function Stat({ label, value }: { label: string; value: number | string }) { return <div className="rounded-lg border bg-muted/30 p-3"><p className="text-xs text-muted-foreground">{label}</p><p className="mt-1 text-xl font-semibold tabular-nums">{value}</p></div>; }
