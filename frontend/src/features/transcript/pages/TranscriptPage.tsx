import { useMemo, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { Download, Loader2, TrendingUp } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useAcademicYears } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { useStudentTranscript } from "../hooks/useTranscriptData";

export function TranscriptPage() {
  const { id } = useParams();
  const studentId = Number(id);
  const { hasPermission } = usePermissions();
  const { data: academicYears = [] } = useAcademicYears();
  const [selectedYearId, setSelectedYearId] = useState<string>("");

  const transcriptQuery = useStudentTranscript(Number.isNaN(studentId) ? null : studentId, selectedYearId ? Number(selectedYearId) : undefined);

  const selectedYearName = useMemo(
    () => academicYears.find((item) => String(item.id) === selectedYearId)?.name ?? "All academic years",
    [academicYears, selectedYearId]
  );

  if (!hasPermission(Permission.TranscriptView)) {
    return (
      <Card>
        <CardContent className="p-6 text-sm text-destructive">
          You do not have permission to view transcripts.
        </CardContent>
      </Card>
    );
  }

  if (transcriptQuery.isPending) {
    return (
      <div className="mx-auto max-w-6xl py-12 text-center text-sm text-muted-foreground">
        <Loader2 className="mr-2 inline h-4 w-4 animate-spin" />
        Loading transcript…
      </div>
    );
  }

  if (transcriptQuery.isError || !transcriptQuery.data) {
    return (
      <Card className="mx-auto max-w-6xl">
        <CardContent className="p-6 text-sm text-destructive">
          Unable to load the transcript for this student.
        </CardContent>
      </Card>
    );
  }

  const { data: transcript } = transcriptQuery;

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Student transcript</h1>
          <p className="text-sm text-muted-foreground">
            Academic summary and historical result overview for {transcript.studentName}.
          </p>
        </div>
        <div className="flex flex-wrap gap-2">
          <Link to={`/students/${studentId}`} className="inline-flex items-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent">
            Back to profile
          </Link>
          <Button variant="outline" type="button">
            <Download className="mr-2 h-4 w-4" />
            Export PDF
          </Button>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Academic filter</CardTitle>
          <CardDescription>Select a year to view a specific transcript.</CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-3 md:flex-row">
          <select
            value={selectedYearId}
            onChange={(event) => setSelectedYearId(event.target.value)}
            className="rounded-md border bg-background px-3 py-2 md:max-w-xs"
          >
            <option value="">All academic years</option>
            {academicYears.map((year) => (
              <option key={year.id} value={year.id}>
                {year.name}
              </option>
            ))}
          </select>
          <div className="rounded-md border bg-muted/40 px-3 py-2 text-sm text-muted-foreground md:ml-auto">
            {selectedYearName}
          </div>
        </CardContent>
      </Card>

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <Stat label="Final GPA" value={transcript.summary.finalGpa.toFixed(2)} />
        <Stat label="Final grade" value={transcript.summary.finalGrade || "—"} />
        <Stat label="Attendance" value={`${transcript.attendanceSummary.percentage.toFixed(1)}%`} />
        <Stat label="Position" value={transcript.summary.position ? `#${transcript.summary.position}` : "—"} />
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Student details</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3 text-sm">
            <div className="flex items-center justify-between border-b pb-2">
              <span className="text-muted-foreground">Student</span>
              <span className="font-medium">{transcript.studentName}</span>
            </div>
            <div className="flex items-center justify-between border-b pb-2">
              <span className="text-muted-foreground">Roll</span>
              <span className="font-medium">{transcript.rollNo}</span>
            </div>
            <div className="flex items-center justify-between border-b pb-2">
              <span className="text-muted-foreground">Class</span>
              <span className="font-medium">{transcript.className}</span>
            </div>
            <div className="flex items-center justify-between border-b pb-2">
              <span className="text-muted-foreground">Section</span>
              <span className="font-medium">{transcript.sectionName}</span>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-muted-foreground">Generated</span>
              <span className="font-medium">{new Date(transcript.generatedAt).toLocaleString()}</span>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Year trend</CardTitle>
            <CardDescription>GPA history across the academic years.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-3">
            {transcript.gpaHistory.length ? (
              transcript.gpaHistory.map((point) => (
                <div key={point.academicYearId} className="flex items-center justify-between rounded-lg border p-3">
                  <span>{point.academicYearName}</span>
                  <span className="inline-flex items-center gap-1 font-medium text-primary">
                    <TrendingUp className="h-4 w-4" />
                    {point.gpa.toFixed(2)}
                  </span>
                </div>
              ))
            ) : (
              <p className="text-sm text-muted-foreground">No GPA history available yet.</p>
            )}
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Exam history</CardTitle>
        </CardHeader>
        <CardContent className="overflow-x-auto">
          <table className="w-full min-w-[700px] text-sm">
            <thead>
              <tr className="border-b text-left">
                <th className="p-2">Exam</th>
                <th className="p-2">Type</th>
                <th className="p-2">Total</th>
                <th className="p-2">% </th>
                <th className="p-2">GPA</th>
                <th className="p-2">Grade</th>
                <th className="p-2">Result</th>
              </tr>
            </thead>
            <tbody>
              {transcript.examHistory.map((exam) => (
                <tr key={exam.examId} className="border-b">
                  <td className="p-2 font-medium">{exam.examName}</td>
                  <td className="p-2">{exam.examTypeName ?? "—"}</td>
                  <td className="p-2 tabular-nums">{exam.totalMarks}</td>
                  <td className="p-2 tabular-nums">{exam.percentage.toFixed(1)}%</td>
                  <td className="p-2 tabular-nums">{exam.gpa.toFixed(2)}</td>
                  <td className="p-2">{exam.grade}</td>
                  <td className="p-2">{exam.isPassed ? "Passed" : "Failed"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Academic year summary</CardTitle>
        </CardHeader>
        <CardContent className="overflow-x-auto">
          <table className="w-full min-w-[600px] text-sm">
            <thead>
              <tr className="border-b text-left">
                <th className="p-2">Year</th>
                <th className="p-2">Final GPA</th>
                <th className="p-2">Grade</th>
                <th className="p-2">Total marks</th>
                <th className="p-2">Position</th>
              </tr>
            </thead>
            <tbody>
              {transcript.yearSummaries.map((year) => (
                <tr key={year.academicYearId} className="border-b">
                  <td className="p-2">{year.academicYearName}</td>
                  <td className="p-2 tabular-nums">{year.finalGpa.toFixed(2)}</td>
                  <td className="p-2">{year.finalGrade}</td>
                  <td className="p-2 tabular-nums">{year.totalMarks}</td>
                  <td className="p-2">{year.position ? `#${year.position}` : "—"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </CardContent>
      </Card>
    </div>
  );
}

function Stat({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-lg border bg-muted/30 p-4">
      <p className="text-xs text-muted-foreground">{label}</p>
      <p className="mt-2 text-2xl font-semibold tabular-nums">{value}</p>
    </div>
  );
}
