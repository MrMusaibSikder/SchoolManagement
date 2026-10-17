import { useMemo, useState } from "react";
import { Link, useParams } from "react-router-dom";
import {
  ArrowLeft,
  Download,
  Loader2,
  RefreshCw,
  TrendingUp,
} from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { useAcademicYears } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { getApiErrorMessage } from "@/lib/api/error-message";
import { Permission } from "@/lib/permissions";
import { useStudentTranscript } from "../hooks/useTranscriptData";
import { downloadStudentTranscriptPdf } from "../api/transcript.api";
import type {
  PromotionStatus,
  TranscriptSubjectDto,
} from "../types/transcript.types";

const promotionLabels: Record<PromotionStatus, string> = {
  1: "Pending",
  2: "Promoted",
  3: "Not promoted",
};

function formatDate(value: string | null | undefined): string {
  if (!value) return "—";
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "—";

  return new Intl.DateTimeFormat("en-BD", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  }).format(date);
}

function formatGpa(value: number | null | undefined): string {
  return typeof value === "number" && Number.isFinite(value)
    ? value.toFixed(2)
    : "—";
}

function SubjectResults({ subjects }: { subjects: TranscriptSubjectDto[] }) {
  if (subjects.length === 0) {
    return (
      <p className="text-sm text-muted-foreground">
        No subject breakdown is available.
      </p>
    );
  }

  return (
    <div className="grid gap-2 sm:grid-cols-2">
      {subjects.map((subject) => (
        <div
          key={subject.subjectId}
          className="flex min-w-0 items-center justify-between gap-3 rounded-lg border bg-background/70 p-3"
        >
          <div className="min-w-0">
            <p className="truncate text-sm font-medium">{subject.subjectName}</p>
            <p className="mt-1 text-xs text-muted-foreground">
              {subject.marksObtained} marks
              {subject.isOptional ? " · Optional" : ""}
            </p>
          </div>
          <div className="shrink-0 text-right">
            <p className="text-sm font-semibold">{subject.grade || "—"}</p>
            <p className="text-xs text-muted-foreground">
              GPA {formatGpa(subject.gpa)}
            </p>
          </div>
        </div>
      ))}
    </div>
  );
}

export function TranscriptPage() {
  const { id } = useParams();
  const studentId = Number(id);
  const validStudentId = Number.isInteger(studentId) && studentId > 0;
  const {
    hasPermission,
    isPending: permissionsPending,
  } = usePermissions();
  const { data: academicYears = [] } = useAcademicYears();
  const [selectedYearId, setSelectedYearId] = useState("");
  const [isDownloading, setIsDownloading] = useState(false);

  const transcriptQuery = useStudentTranscript(
    validStudentId ? studentId : null,
    selectedYearId ? Number(selectedYearId) : undefined
  );
  const selectedYearName = useMemo(
    () =>
      academicYears.find((item) => String(item.id) === selectedYearId)?.name ??
      "All academic years",
    [academicYears, selectedYearId]
  );

  async function exportTranscript() {
    if (!validStudentId || isDownloading) return;
    setIsDownloading(true);

    try {
      const file = await downloadStudentTranscriptPdf(
        studentId,
        selectedYearId ? Number(selectedYearId) : undefined
      );
      const objectUrl = URL.createObjectURL(file);
      const anchor = document.createElement("a");
      anchor.href = objectUrl;
      anchor.download = `Transcript-${studentId}${selectedYearId ? `-${selectedYearId}` : ""}.pdf`;
      document.body.appendChild(anchor);
      anchor.click();
      anchor.remove();
      window.setTimeout(() => URL.revokeObjectURL(objectUrl), 1000);
      toast.success("Transcript PDF downloaded.");
    } catch (error) {
      toast.error(getApiErrorMessage(error, "Could not download the transcript."));
    } finally {
      setIsDownloading(false);
    }
  }

  if (permissionsPending) {
    return (
      <div className="flex min-h-48 items-center justify-center text-sm text-muted-foreground">
        <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
        Checking transcript access…
      </div>
    );
  }

  if (!hasPermission(Permission.TranscriptView)) {
    return (
      <Card>
        <CardContent className="p-6 text-sm text-destructive">
          You do not have permission to view transcripts.
        </CardContent>
      </Card>
    );
  }

  if (!validStudentId) {
    return (
      <Card>
        <CardContent className="p-6 text-sm text-destructive">
          The student ID in this link is invalid.
        </CardContent>
      </Card>
    );
  }

  if (transcriptQuery.isPending) {
    return (
      <div
        className="mx-auto max-w-6xl py-12 text-center text-sm text-muted-foreground"
        aria-label="Loading transcript"
      >
        <Loader2 aria-hidden="true" className="mr-2 inline h-4 w-4 animate-spin" />
        Loading transcript…
      </div>
    );
  }

  if (transcriptQuery.isError || !transcriptQuery.data) {
    return (
      <Card className="mx-auto max-w-6xl">
        <CardContent className="flex flex-col items-start gap-3 p-6">
          <div>
            <h1 className="font-semibold">Transcript unavailable</h1>
            <p className="mt-1 text-sm text-muted-foreground">
              We could not load this student’s academic history.
            </p>
          </div>
          <Button
            type="button"
            variant="outline"
            onClick={() => void transcriptQuery.refetch()}
            disabled={transcriptQuery.isFetching}
          >
            {transcriptQuery.isFetching ? (
              <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
            ) : (
              <RefreshCw aria-hidden="true" className="mr-2 h-4 w-4" />
            )}
            Try again
          </Button>
        </CardContent>
      </Card>
    );
  }

  const transcript = transcriptQuery.data;
  const latestYear = transcript.yearSummaries.at(-1);

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <Link
            to={`/students/${studentId}`}
            className="mb-3 inline-flex items-center gap-2 text-sm font-medium text-muted-foreground transition-colors hover:text-foreground"
          >
            <ArrowLeft aria-hidden="true" className="h-4 w-4" />
            Back to student profile
          </Link>
          <h1 className="font-display text-2xl font-semibold">
            Academic transcript
          </h1>
          <p className="mt-1 text-sm text-muted-foreground">
            Published exam and year-end results for {transcript.studentName}.
          </p>
        </div>
        <Button
          type="button"
          variant="outline"
          onClick={() => void exportTranscript()}
          disabled={isDownloading}
        >
          {isDownloading ? (
            <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
          ) : (
            <Download aria-hidden="true" className="mr-2 h-4 w-4" />
          )}
          {isDownloading ? "Preparing PDF…" : "Export PDF"}
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Academic year</CardTitle>
          <CardDescription>
            View the complete history or narrow this transcript to one year.
          </CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-3 sm:flex-row sm:items-center">
          <label htmlFor="transcript-academic-year" className="sr-only">
            Select academic year
          </label>
          <select
            id="transcript-academic-year"
            value={selectedYearId}
            onChange={(event) => setSelectedYearId(event.target.value)}
            className="h-10 w-full rounded-md border border-input bg-background px-3 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring sm:max-w-xs"
          >
            <option value="">All academic years</option>
            {academicYears.map((year) => (
              <option key={year.id} value={year.id}>
                {year.name}
              </option>
            ))}
          </select>
          <p className="text-sm text-muted-foreground sm:ml-auto">
            Showing: {selectedYearName}
          </p>
        </CardContent>
      </Card>

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <Stat label="Cumulative GPA" value={formatGpa(transcript.summary.cgpa)} />
        <Stat
          label="Published exams"
          value={transcript.summary.totalExamsIncluded.toLocaleString("en-BD")}
        />
        <Stat
          label="Academic years"
          value={transcript.summary.totalAcademicYearsIncluded.toLocaleString("en-BD")}
        />
        <Stat
          label="Attendance"
          value={`${formatGpa(transcript.attendanceSummary.attendancePercentage)}%`}
        />
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Student details</CardTitle>
            <CardDescription>
              Transcript generated {formatDate(transcript.generatedAt)}.
            </CardDescription>
          </CardHeader>
          <CardContent className="grid gap-4 text-sm sm:grid-cols-2">
            <Detail label="Student" value={transcript.studentName} />
            <Detail label="Roll number" value={transcript.rollNo} />
            <Detail label="Class" value={transcript.className} />
            <Detail label="Section" value={transcript.sectionName} />
            <Detail
              label="Overall result"
              value={transcript.summary.overallPassed ? "Passed" : "Not passed"}
            />
            <Detail
              label="Latest promotion"
              value={
                transcript.summary.latestPromotionStatus
                  ? promotionLabels[transcript.summary.latestPromotionStatus]
                  : "—"
              }
            />
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Academic trends</CardTitle>
            <CardDescription>
              GPA and class/section/merit positions by academic year.
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-3">
            {transcript.gpaHistory.length > 0 ? (
              transcript.gpaHistory.map((point) => {
                const position = transcript.positionHistory.find(
                  (entry) => entry.academicYearName === point.label
                );

                return (
                  <div
                    key={point.label}
                    className="rounded-lg border bg-muted/20 p-3"
                  >
                    <div className="flex flex-wrap items-center justify-between gap-2">
                      <span className="font-medium">{point.label}</span>
                      <span className="inline-flex items-center gap-1 font-semibold text-primary">
                        <TrendingUp aria-hidden="true" className="h-4 w-4" />
                        GPA {formatGpa(point.gpa)}
                      </span>
                    </div>
                    {position ? (
                      <div className="mt-2 flex flex-wrap gap-x-4 gap-y-1 text-xs text-muted-foreground">
                        <span>Class: {position.classPosition ?? "—"}</span>
                        <span>Section: {position.sectionPosition ?? "—"}</span>
                        <span>Merit: {position.meritPosition ?? "—"}</span>
                      </div>
                    ) : null}
                  </div>
                );
              })
            ) : (
              <EmptyState message="No academic trend is available yet." />
            )}
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Attendance summary</CardTitle>
          <CardDescription>
            Attendance records included in this transcript.
          </CardDescription>
        </CardHeader>
        <CardContent className="grid grid-cols-2 gap-3 sm:grid-cols-3 lg:grid-cols-6">
          <AttendanceStat label="Present" value={transcript.attendanceSummary.presentDays} />
          <AttendanceStat label="Absent" value={transcript.attendanceSummary.absentDays} />
          <AttendanceStat label="Late" value={transcript.attendanceSummary.lateDays} />
          <AttendanceStat label="Leave" value={transcript.attendanceSummary.leaveDays} />
          <AttendanceStat label="Total days" value={transcript.attendanceSummary.totalDays} />
          <AttendanceStat label="Attendance" value={`${formatGpa(transcript.attendanceSummary.attendancePercentage)}%`} />
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Exam history</CardTitle>
          <CardDescription>
            Published exam outcomes with subject-level results.
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          {transcript.examHistory.length > 0 ? (
            transcript.examHistory.map((exam) => (
              <article
                key={exam.examId}
                className="rounded-xl border bg-muted/10 p-4 sm:p-5"
              >
                <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                  <div>
                    <h3 className="font-semibold">{exam.examName}</h3>
                    <p className="mt-1 text-sm text-muted-foreground">
                      {exam.examTypeName} · {exam.academicYearName}
                    </p>
                  </div>
                  <span
                    className={`w-fit rounded-full px-3 py-1 text-xs font-semibold ${
                      exam.isPassed
                        ? "bg-emerald-100 text-emerald-800"
                        : "bg-rose-100 text-rose-800"
                    }`}
                  >
                    {exam.isPassed ? "Passed" : "Not passed"}
                  </span>
                </div>
                <div className="my-4 grid grid-cols-2 gap-3 sm:grid-cols-4">
                  <InlineMetric label="GPA" value={formatGpa(exam.gpa)} />
                  <InlineMetric label="Grade" value={exam.grade || "—"} />
                  <InlineMetric label="Subjects" value={exam.subjects.length.toLocaleString("en-BD")} />
                  <InlineMetric label="Published" value={formatDate(exam.publishedAt)} />
                </div>
                <SubjectResults subjects={exam.subjects} />
              </article>
            ))
          ) : (
            <EmptyState message="No published exam results are available for this selection." />
          )}
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Academic year results</CardTitle>
          <CardDescription>
            Published year-end outcomes, promotion status, and subject results.
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          {transcript.yearSummaries.length > 0 ? (
            transcript.yearSummaries.map((year) => (
              <article
                key={year.academicYearId}
                className="rounded-xl border bg-muted/10 p-4 sm:p-5"
              >
                <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                  <div>
                    <h3 className="font-semibold">{year.academicYearName}</h3>
                    <p className="mt-1 text-sm text-muted-foreground">
                      Published {formatDate(year.publishedAt)}
                    </p>
                  </div>
                  <span
                    className={`w-fit rounded-full px-3 py-1 text-xs font-semibold ${
                      year.isPassed
                        ? "bg-emerald-100 text-emerald-800"
                        : "bg-rose-100 text-rose-800"
                    }`}
                  >
                    {year.isPassed ? "Passed" : "Not passed"}
                  </span>
                </div>
                <div className="my-4 grid grid-cols-2 gap-3 sm:grid-cols-4">
                  <InlineMetric label="Final GPA" value={formatGpa(year.finalGpa)} />
                  <InlineMetric label="Grade" value={year.finalGrade || "—"} />
                  <InlineMetric
                    label="Promotion"
                    value={promotionLabels[year.promotionStatus] ?? "—"}
                  />
                  <InlineMetric label="Class position" value={year.classPosition ?? "—"} />
                </div>
                {year.teacherRemarks || year.principalRemarks ? (
                  <div className="mb-4 grid gap-3 sm:grid-cols-2">
                    {year.teacherRemarks ? (
                      <Remark label="Teacher remarks" value={year.teacherRemarks} />
                    ) : null}
                    {year.principalRemarks ? (
                      <Remark label="Principal remarks" value={year.principalRemarks} />
                    ) : null}
                  </div>
                ) : null}
                <SubjectResults subjects={year.subjects} />
              </article>
            ))
          ) : (
            <EmptyState message="No published year-end results are available for this selection." />
          )}
        </CardContent>
      </Card>

      {latestYear ? (
        <p className="text-right text-xs text-muted-foreground">
          Latest academic year: {latestYear.academicYearName}
        </p>
      ) : null}
    </div>
  );
}

function Stat({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-xl border bg-gradient-to-br from-primary/5 to-background p-4">
      <p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
        {label}
      </p>
      <p className="mt-2 text-2xl font-semibold tabular-nums">{value}</p>
    </div>
  );
}

function Detail({ label, value }: { label: string; value: string }) {
  return (
    <div className="min-w-0 border-b pb-2">
      <p className="text-xs text-muted-foreground">{label}</p>
      <p className="mt-1 break-words font-medium">{value}</p>
    </div>
  );
}

function InlineMetric({
  label,
  value,
}: {
  label: string;
  value: string | number;
}) {
  return (
    <div className="rounded-lg border bg-background/80 p-3">
      <p className="text-xs text-muted-foreground">{label}</p>
      <p className="mt-1 break-words text-sm font-semibold">{value}</p>
    </div>
  );
}

function AttendanceStat({
  label,
  value,
}: {
  label: string;
  value: string | number;
}) {
  return (
    <div className="rounded-lg border bg-muted/20 p-3 text-center">
      <p className="text-xs text-muted-foreground">{label}</p>
      <p className="mt-1 font-semibold tabular-nums">{value}</p>
    </div>
  );
}

function Remark({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-lg border border-primary/10 bg-primary/5 p-3">
      <p className="text-xs font-medium text-muted-foreground">{label}</p>
      <p className="mt-1 whitespace-pre-wrap text-sm">{value}</p>
    </div>
  );
}

function EmptyState({ message }: { message: string }) {
  return (
    <p className="rounded-lg border border-dashed p-5 text-center text-sm text-muted-foreground">
      {message}
    </p>
  );
}
