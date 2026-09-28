import { useMemo, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { Loader2 } from "lucide-react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useAcademicYears } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { useStudentProgressReport } from "@/features/transcript/hooks/useTranscriptData";

export function ProgressReportPage() {
  const { id } = useParams();
  const studentId = Number(id);
  const { hasPermission } = usePermissions();
  const { data: academicYears = [] } = useAcademicYears();
  const [selectedYearId, setSelectedYearId] = useState<string>(academicYears[0]?.id ? String(academicYears[0].id) : "");

  const progressQuery = useStudentProgressReport(
    Number.isNaN(studentId) ? null : studentId,
    selectedYearId ? Number(selectedYearId) : null
  );

  const selectedYearName = useMemo(
    () => academicYears.find((year) => String(year.id) === selectedYearId)?.name ?? "Academic year",
    [academicYears, selectedYearId]
  );

  if (!hasPermission(Permission.ProgressReportView)) {
    return (
      <Card>
        <CardContent className="p-6 text-sm text-destructive">
          You do not have permission to view progress reports.
        </CardContent>
      </Card>
    );
  }

  if (progressQuery.isPending) {
    return (
      <div className="mx-auto max-w-7xl py-12 text-center text-sm text-muted-foreground">
        <Loader2 className="mr-2 inline h-4 w-4 animate-spin" />
        Loading progress report…
      </div>
    );
  }

  if (progressQuery.isError || !progressQuery.data) {
    return (
      <Card className="mx-auto max-w-7xl">
        <CardContent className="p-6 text-sm text-destructive">
          Unable to load the progress report.
        </CardContent>
      </Card>
    );
  }

  const { data: report } = progressQuery;

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Progress report</h1>
          <p className="text-sm text-muted-foreground">
            Subject-wise performance for {report.studentName} in {report.academicYearName}.
          </p>
        </div>
        <Link to={`/students/${studentId}`} className="inline-flex items-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent">
          Back to profile
        </Link>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Academic year</CardTitle>
          <CardDescription>Select the year to review the report.</CardDescription>
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
          <div className="rounded-md border bg-muted/40 px-3 py-2 text-sm text-muted-foreground md:ml-auto">
            {selectedYearName}
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>{report.studentName}</CardTitle>
          <CardDescription>
            {report.className} • {report.sectionName} • Roll {report.rollNo}
          </CardDescription>
        </CardHeader>
        <CardContent className="overflow-x-auto">
          <table className="w-full min-w-[700px] text-sm">
            <thead>
              <tr className="border-b text-left">
                <th className="p-2">Subject</th>
                {report.exams.map((exam) => (
                  <th key={exam.examId} className="p-2">{exam.examName}</th>
                ))}
                <th className="p-2">Average</th>
              </tr>
            </thead>
            <tbody>
              {report.subjects.map((subject) => (
                <tr key={subject.subjectId} className="border-b">
                  <td className="p-2 font-medium">{subject.subjectName}</td>
                  {subject.examMarks.map((mark, index) => (
                    <td key={`${subject.subjectId}-${index}`} className="p-2 tabular-nums">
                      {mark ?? "—"}
                    </td>
                  ))}
                  <td className="p-2 tabular-nums">{subject.average ?? "—"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </CardContent>
      </Card>
    </div>
  );
}
