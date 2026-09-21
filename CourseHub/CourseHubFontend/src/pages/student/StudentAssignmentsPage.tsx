import { useEffect, useState } from "react";
import { api, ApiError } from "../../lib/api";
import type { MyAssignmentResponse, PublicCourseResponse } from "../../lib/types";
import { Badge, Button, Card, EmptyState, ErrorBanner, Field, PageHeader, Spinner, Textarea } from "../../components/ui";

export default function StudentAssignmentsPage() {
  const [assignments, setAssignments] = useState<MyAssignmentResponse[] | null>(null);
  const [courses, setCourses] = useState<PublicCourseResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [submittingId, setSubmittingId] = useState<string | null>(null);
  const [textContent, setTextContent] = useState("");
  const [file, setFile] = useState<File | null>(null);
  const [isSaving, setIsSaving] = useState(false);

  async function load() {
    setIsLoading(true);
    try {
      const result = await api.get<MyAssignmentResponse[]>("/api/me/assignments");
      setAssignments(result);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load assignments.");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    load();
    api
      .get<PublicCourseResponse[]>("/api/public/courses")
      .then(setCourses)
      .catch(() => {});
  }, []);

  function courseName(id: string) {
    return courses.find((c) => c.id === id)?.name ?? "Course";
  }

  function startSubmit(id: string) {
    setSubmittingId(id);
    setTextContent("");
    setFile(null);
    setError(null);
  }

  async function handleSubmit(id: string) {
    if (!textContent.trim() && !file) {
      setError("Add some text or attach a file before submitting.");
      return;
    }
    setIsSaving(true);
    setError(null);
    try {
      const formData = new FormData();
      if (file) {
        formData.append("File", file);
      } else {
        formData.append("TextContent", textContent);
      }
      await api.postForm(`/api/me/assignments/${id}/submit`, formData);
      setSubmittingId(null);
      load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to submit assignment.");
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <div>
      <PageHeader title="My assignments" subtitle="Assignments from the courses you're enrolled in" />
      <ErrorBanner message={error} />
      {isLoading ? (
        <Spinner />
      ) : !assignments || assignments.length === 0 ? (
        <EmptyState message="No assignments right now." />
      ) : (
        <div className="space-y-4">
          {assignments.map((a) => {
            const sub = a.mySubmission;
            const isPastDue = new Date(a.dueDate).getTime() < Date.now();
            return (
              <Card key={a.id} className="p-5">
                <div className="flex flex-wrap items-start justify-between gap-3">
                  <div>
                    <p className="font-display text-sm font-semibold text-ink">{a.title}</p>
                    <p className="mt-0.5 text-xs text-slate-400">
                      {courseName(a.courseId)} · Due {new Date(a.dueDate).toLocaleString()} · {a.maxMarks} marks
                    </p>
                  </div>
                  {!sub ? (
                    <Badge tone={isPastDue ? "rose" : "amber"}>{isPastDue ? "Overdue" : "Not submitted"}</Badge>
                  ) : (
                    <Badge tone={sub.status === "Graded" ? "green" : "blue"}>{sub.status}</Badge>
                  )}
                </div>

                {a.description && <p className="mt-2 text-sm text-slate-500">{a.description}</p>}

                {sub ? (
                  <div className="mt-3 rounded-lg bg-slate-50 p-3 text-sm">
                    <p className="text-xs text-slate-400">
                      Submitted {new Date(sub.submittedAt).toLocaleString()}
                      {sub.isLate && " · Late"}
                    </p>
                    {sub.submissionType === "Text" && sub.textContent && (
                      <p className="mt-1 whitespace-pre-wrap text-slate-600">{sub.textContent}</p>
                    )}
                    {sub.submissionType === "File" && sub.originalFileName && (
                      <p className="mt-1 text-slate-600">📎 {sub.originalFileName}</p>
                    )}
                    {sub.status === "Graded" && (
                      <div className="mt-2 border-t border-slate-200 pt-2">
                        <p className="font-medium text-ink">
                          Grade: {sub.marks} / {a.maxMarks}
                        </p>
                        {sub.feedback && <p className="mt-1 text-slate-500">{sub.feedback}</p>}
                      </div>
                    )}
                  </div>
                ) : submittingId === a.id ? (
                  <div className="mt-3 space-y-3 border-t border-slate-100 pt-3">
                    <Field label="Write your answer">
                      <Textarea
                        rows={4}
                        value={textContent}
                        disabled={!!file}
                        onChange={(e) => setTextContent(e.target.value)}
                        placeholder="Type your submission here…"
                      />
                    </Field>
                    <Field label="…or attach a file instead">
                      <input
                        type="file"
                        onChange={(e) => setFile(e.target.files?.[0] ?? null)}
                        className="block w-full text-sm text-slate-500 file:mr-3 file:rounded-lg file:border-0 file:bg-brand-50 file:px-3 file:py-2 file:text-sm file:font-medium file:text-brand-700 hover:file:bg-brand-100"
                      />
                    </Field>
                    <div className="flex justify-end gap-2">
                      <Button variant="secondary" onClick={() => setSubmittingId(null)}>
                        Cancel
                      </Button>
                      <Button onClick={() => handleSubmit(a.id)} disabled={isSaving}>
                        {isSaving ? "Submitting…" : "Submit"}
                      </Button>
                    </div>
                  </div>
                ) : (
                  <div className="mt-3">
                    <Button onClick={() => startSubmit(a.id)}>Submit assignment</Button>
                  </div>
                )}
              </Card>
            );
          })}
        </div>
      )}
    </div>
  );
}
