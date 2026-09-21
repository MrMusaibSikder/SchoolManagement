import { useEffect, useState } from "react";
import { api, ApiError } from "../../lib/api";
import { useAuth } from "../../lib/auth";
import type {
  AssignmentResponse,
  CourseResponse,
  PagedResult,
  RosterEntryResponse,
} from "../../lib/types";
import {
  Badge,
  Button,
  Card,
  EmptyState,
  ErrorBanner,
  Field,
  Input,
  Modal,
  PageHeader,
  Spinner,
  Textarea,
} from "../../components/ui";

const emptyForm = { courseId: "", title: "", description: "", maxMarks: 100, dueDate: "" };
const emptyGradeForm = { marks: 0, feedback: "" };

/** "2026-09-30T18:30:00Z" -> "2026-09-30T18:30" for an <input type="datetime-local">. */
function toLocalInputValue(iso: string): string {
  const d = new Date(iso);
  const pad = (n: number) => String(n).padStart(2, "0");
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

function triggerBrowserDownload(blob: Blob, filename: string) {
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = filename;
  document.body.appendChild(a);
  a.click();
  a.remove();
  URL.revokeObjectURL(url);
}

export default function AssignmentsPage() {
  const { hasPermission } = useAuth();
  const [data, setData] = useState<PagedResult<AssignmentResponse> | null>(null);
  const [courses, setCourses] = useState<CourseResponse[]>([]);
  const [courseFilter, setCourseFilter] = useState("");
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<AssignmentResponse | null>(null);
  const [form, setForm] = useState(emptyForm);

  const [rosterFor, setRosterFor] = useState<AssignmentResponse | null>(null);
  const [roster, setRoster] = useState<RosterEntryResponse[] | null>(null);
  const [rosterLoading, setRosterLoading] = useState(false);
  const [rosterError, setRosterError] = useState<string | null>(null);
  const [gradingSubmissionId, setGradingSubmissionId] = useState<string | null>(null);
  const [gradeForm, setGradeForm] = useState(emptyGradeForm);
  const [downloadingId, setDownloadingId] = useState<string | null>(null);

  useEffect(() => {
    // Teachers don't necessarily have every permission the course list
    // touches — same "fail silently" pattern as EnrollmentsPage's
    // students/batches lookups.
    api
      .get<PagedResult<CourseResponse>>("/api/admin/courses", { pageSize: 200 })
      .then((r) => setCourses(r.items))
      .catch(() => {});
  }, []);

  async function load(pageOverride?: number) {
    setIsLoading(true);
    try {
      const result = await api.get<PagedResult<AssignmentResponse>>("/api/admin/assignments", {
        courseId: courseFilter || undefined,
        page: pageOverride ?? page,
        pageSize: 10,
      });
      setData(result);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load assignments.");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, courseFilter]);

  function courseName(id: string) {
    return courses.find((c) => c.id === id)?.name ?? id.slice(0, 8);
  }

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setModalOpen(true);
  }

  function openEdit(a: AssignmentResponse) {
    setEditing(a);
    setForm({
      courseId: a.courseId,
      title: a.title,
      description: a.description ?? "",
      maxMarks: a.maxMarks,
      dueDate: toLocalInputValue(a.dueDate),
    });
    setModalOpen(true);
  }

  async function handleSubmit() {
    setError(null);
    try {
      const dueDateIso = new Date(form.dueDate).toISOString();
      if (editing) {
        await api.put(`/api/admin/assignments/${editing.id}`, {
          title: form.title,
          description: form.description || null,
          maxMarks: form.maxMarks,
          dueDate: dueDateIso,
        });
      } else {
        await api.post("/api/admin/assignments", {
          courseId: form.courseId,
          createdByTeacherId: null,
          title: form.title,
          description: form.description || null,
          maxMarks: form.maxMarks,
          dueDate: dueDateIso,
        });
      }
      setModalOpen(false);
      setPage(editing ? page : 1);
      load(editing ? undefined : 1);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to save assignment.");
    }
  }

  async function runAction(action: (id: string) => Promise<unknown>, id: string) {
    setError(null);
    try {
      await action(id);
      load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Action failed.");
    }
  }

  async function openRoster(a: AssignmentResponse) {
    setRosterFor(a);
    setRoster(null);
    setRosterError(null);
    setGradingSubmissionId(null);
    setRosterLoading(true);
    try {
      const result = await api.get<RosterEntryResponse[]>(`/api/admin/assignments/${a.id}/roster`);
      setRoster(result);
    } catch (err) {
      setRosterError(err instanceof ApiError ? err.message : "Failed to load roster.");
    } finally {
      setRosterLoading(false);
    }
  }

  function startGrading(entry: RosterEntryResponse) {
    if (!entry.submission) return;
    setGradingSubmissionId(entry.submission.id);
    setGradeForm({ marks: entry.submission.marks ?? 0, feedback: entry.submission.feedback ?? "" });
  }

  async function submitGrade(submissionId: string) {
    setRosterError(null);
    try {
      await api.post(`/api/admin/submissions/${submissionId}/grade`, {
        marks: gradeForm.marks,
        feedback: gradeForm.feedback || null,
      });
      setGradingSubmissionId(null);
      if (rosterFor) openRoster(rosterFor);
    } catch (err) {
      setRosterError(err instanceof ApiError ? err.message : "Failed to save grade.");
    }
  }

  async function downloadSubmissionFile(submissionId: string) {
    setRosterError(null);
    setDownloadingId(submissionId);
    try {
      const { blob, filename } = await api.downloadFile(`/api/admin/submissions/${submissionId}/file`);
      triggerBrowserDownload(blob, filename);
    } catch (err) {
      setRosterError(err instanceof ApiError ? err.message : "Failed to download file.");
    } finally {
      setDownloadingId(null);
    }
  }

  const canCreate = hasPermission("assignments.create");
  const canUpdate = hasPermission("assignments.update");
  const canGrade = hasPermission("submissions.grade");

  return (
    <div>
      <PageHeader
        title="Assignments"
        subtitle="Create assignments and grade student submissions"
        action={canCreate && <Button onClick={openCreate}>+ New assignment</Button>}
      />
      <ErrorBanner message={error} />
      <div className="mb-4">
        <select
          value={courseFilter}
          onChange={(e) => {
            setCourseFilter(e.target.value);
            setPage(1);
          }}
          className="rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-100"
        >
          <option value="">All courses</option>
          {courses.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>
      </div>
      <Card>
        {isLoading ? (
          <Spinner />
        ) : !data || data.items.length === 0 ? (
          <EmptyState message="No assignments yet." />
        ) : (
          <table className="w-full text-left text-sm">
            <thead className="border-b border-slate-100 text-xs uppercase tracking-wide text-slate-400">
              <tr>
                <th className="px-5 py-3">Title</th>
                <th className="px-5 py-3">Course</th>
                <th className="px-5 py-3">Max marks</th>
                <th className="px-5 py-3">Due date</th>
                <th className="px-5 py-3">Status</th>
                <th className="px-5 py-3 text-right">Actions</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map((a) => (
                <tr key={a.id} className="border-b border-slate-50 last:border-0">
                  <td className="px-5 py-3 font-medium text-ink">{a.title}</td>
                  <td className="px-5 py-3 text-slate-500">{courseName(a.courseId)}</td>
                  <td className="px-5 py-3 text-slate-500">{a.maxMarks}</td>
                  <td className="px-5 py-3 text-slate-500">{new Date(a.dueDate).toLocaleString()}</td>
                  <td className="px-5 py-3">
                    <Badge tone={a.isActive ? "green" : "slate"}>{a.isActive ? "Active" : "Inactive"}</Badge>
                  </td>
                  <td className="px-5 py-3">
                    <div className="flex justify-end gap-2">
                      {canGrade && (
                        <button onClick={() => openRoster(a)} className="text-xs font-medium text-brand-600 hover:underline">
                          Roster
                        </button>
                      )}
                      {canUpdate && (
                        <>
                          <button onClick={() => openEdit(a)} className="text-xs font-medium text-brand-600 hover:underline">
                            Edit
                          </button>
                          <button
                            onClick={() =>
                              runAction(
                                (id) => (a.isActive ? api.post(`/api/admin/assignments/${id}/deactivate`) : api.post(`/api/admin/assignments/${id}/activate`)),
                                a.id,
                              )
                            }
                            className="text-xs font-medium text-slate-500 hover:underline"
                          >
                            {a.isActive ? "Deactivate" : "Activate"}
                          </button>
                        </>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </Card>
      {data && data.totalPages > 1 && (
        <div className="mt-4 flex items-center justify-center gap-3 text-sm text-slate-500">
          <Button variant="secondary" disabled={page <= 1} onClick={() => setPage((p) => p - 1)}>
            Previous
          </Button>
          <span>
            Page {data.page} of {data.totalPages}
          </span>
          <Button variant="secondary" disabled={page >= data.totalPages} onClick={() => setPage((p) => p + 1)}>
            Next
          </Button>
        </div>
      )}

      {modalOpen && (
        <Modal title={editing ? "Edit assignment" : "New assignment"} onClose={() => setModalOpen(false)}>
          <div className="space-y-3">
            {!editing && (
              <Field label="Course">
                <select
                  value={form.courseId}
                  onChange={(e) => setForm({ ...form, courseId: e.target.value })}
                  className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-100"
                >
                  <option value="">Select a course…</option>
                  {courses.map((c) => (
                    <option key={c.id} value={c.id}>
                      {c.name}
                    </option>
                  ))}
                </select>
              </Field>
            )}
            <Field label="Title">
              <Input value={form.title} onChange={(e) => setForm({ ...form, title: e.target.value })} />
            </Field>
            <Field label="Description">
              <Textarea rows={3} value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
            </Field>
            <Field label="Max marks">
              <Input
                type="number"
                min={1}
                value={form.maxMarks}
                onChange={(e) => setForm({ ...form, maxMarks: Number(e.target.value) })}
              />
            </Field>
            <Field label="Due date">
              <Input
                type="datetime-local"
                value={form.dueDate}
                onChange={(e) => setForm({ ...form, dueDate: e.target.value })}
              />
            </Field>
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="secondary" onClick={() => setModalOpen(false)}>
                Cancel
              </Button>
              <Button onClick={handleSubmit} disabled={!editing && !form.courseId}>
                {editing ? "Save changes" : "Create assignment"}
              </Button>
            </div>
          </div>
        </Modal>
      )}

      {rosterFor && (
        <Modal title={`Roster — ${rosterFor.title}`} onClose={() => setRosterFor(null)}>
          <ErrorBanner message={rosterError} />
          {rosterLoading ? (
            <Spinner />
          ) : !roster || roster.length === 0 ? (
            <EmptyState message="No students enrolled in this course yet." />
          ) : (
            <div className="space-y-3">
              {roster.map((entry) => (
                <div key={entry.studentId} className="rounded-lg border border-slate-200 p-3">
                  <div className="flex items-center justify-between gap-2">
                    <div>
                      <p className="text-sm font-medium text-ink">{entry.studentFullName}</p>
                      {!entry.submission ? (
                        <Badge tone="slate">Not submitted</Badge>
                      ) : (
                        <div className="mt-1 flex flex-wrap items-center gap-1.5">
                          <Badge tone={entry.submission.status === "Graded" ? "green" : "amber"}>
                            {entry.submission.status}
                          </Badge>
                          <Badge tone={entry.submission.isLate ? "rose" : "slate"}>
                            {entry.submission.isLate ? "Late" : "On time"}
                          </Badge>
                          {entry.submission.marks !== null && (
                            <span className="text-xs text-slate-500">
                              {entry.submission.marks} / {rosterFor.maxMarks}
                            </span>
                          )}
                        </div>
                      )}
                    </div>
                    {entry.submission && canGrade && (
                      <div className="flex shrink-0 gap-2">
                        {entry.submission.submissionType === "File" && (
                          <button
                            onClick={() => downloadSubmissionFile(entry.submission!.id)}
                            disabled={downloadingId === entry.submission.id}
                            className="text-xs font-medium text-brand-600 hover:underline disabled:opacity-50"
                          >
                            {downloadingId === entry.submission.id ? "Downloading…" : "Download file"}
                          </button>
                        )}
                        <button
                          onClick={() => startGrading(entry)}
                          className="text-xs font-medium text-brand-600 hover:underline"
                        >
                          Grade
                        </button>
                      </div>
                    )}
                  </div>

                  {entry.submission?.submissionType === "Text" && entry.submission.textContent && (
                    <p className="mt-2 whitespace-pre-wrap rounded-md bg-slate-50 p-2 text-xs text-slate-600">
                      {entry.submission.textContent}
                    </p>
                  )}

                  {entry.submission && gradingSubmissionId === entry.submission.id && (
                    <div className="mt-3 space-y-2 border-t border-slate-100 pt-3">
                      <Field label={`Marks (out of ${rosterFor.maxMarks})`}>
                        <Input
                          type="number"
                          min={0}
                          max={rosterFor.maxMarks}
                          value={gradeForm.marks}
                          onChange={(e) => setGradeForm({ ...gradeForm, marks: Number(e.target.value) })}
                        />
                      </Field>
                      <Field label="Feedback">
                        <Textarea
                          rows={2}
                          value={gradeForm.feedback}
                          onChange={(e) => setGradeForm({ ...gradeForm, feedback: e.target.value })}
                        />
                      </Field>
                      <div className="flex justify-end gap-2">
                        <Button variant="secondary" onClick={() => setGradingSubmissionId(null)}>
                          Cancel
                        </Button>
                        <Button onClick={() => submitGrade(entry.submission!.id)}>Save grade</Button>
                      </div>
                    </div>
                  )}
                </div>
              ))}
            </div>
          )}
        </Modal>
      )}
    </div>
  );
}
