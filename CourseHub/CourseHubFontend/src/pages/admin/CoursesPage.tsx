import { useEffect, useState } from "react";
import { api, ApiError } from "../../lib/api";
import { useAuth } from "../../lib/auth";
import type { CourseResponse, PagedResult, TeacherResponse } from "../../lib/types";
import { Badge, Button, Card, EmptyState, ErrorBanner, Field, Input, Modal, PageHeader, Spinner, Textarea } from "../../components/ui";

const emptyForm = { name: "", code: "", durationInMonths: 1, description: "", thumbnailUrl: "" };

export default function CoursesPage() {
  const { hasPermission } = useAuth();
  const [data, setData] = useState<PagedResult<CourseResponse> | null>(null);
  const [teachers, setTeachers] = useState<TeacherResponse[]>([]);
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<CourseResponse | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [assigningCourse, setAssigningCourse] = useState<CourseResponse | null>(null);
  const [assignTeacherId, setAssignTeacherId] = useState("");

  useEffect(() => {
    // A Teacher can't see this (no teachers.view) — fails silently, same
    // "best effort lookup" pattern as EnrollmentsPage's students/batches.
    api
      .get<PagedResult<TeacherResponse>>("/api/admin/teachers", { pageSize: 200 })
      .then((r) => setTeachers(r.items))
      .catch(() => {});
  }, []);

  async function load(pageOverride?: number) {
    setIsLoading(true);
    try {
      const result = await api.get<PagedResult<CourseResponse>>("/api/admin/courses", {
        search,
        page: pageOverride ?? page,
        pageSize: 10,
      });
      setData(result);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load courses.");
    } finally {
      setIsLoading(false);
    }
  }

  function handleSearchEnter() {
    setPage(1);
    load(1);
  }

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page]);

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setModalOpen(true);
  }

  function openEdit(course: CourseResponse) {
    setEditing(course);
    setForm({
      name: course.name,
      code: course.code,
      durationInMonths: course.durationInMonths,
      description: course.description ?? "",
      thumbnailUrl: course.thumbnailUrl ?? "",
    });
    setModalOpen(true);
  }

  async function handleSubmit() {
    setError(null);
    try {
      const { thumbnailUrl, ...updateFields } = form;
      if (editing) {
        await api.put(`/api/admin/courses/${editing.id}`, updateFields);
        // Separate endpoint on the backend (PUT .../thumbnail) — fired
        // as a second call only when editing an existing course, since a
        // brand-new course has no id yet to attach an image to.
        await api.put(`/api/admin/courses/${editing.id}/thumbnail`, { thumbnailUrl: thumbnailUrl || null });
        setModalOpen(false);
        // Stay on the current page — the course being edited was already
        // visible here, no reason to jump the admin back to page 1.
        load();
      } else {
        await api.post("/api/admin/courses", updateFields);
        setModalOpen(false);
        setPage(1);
        load(1);
      }
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to save course.");
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

  function teacherName(id: string | null) {
    if (!id) return null;
    const t = teachers.find((x) => x.id === id);
    return t ? `${t.firstName} ${t.lastName}` : id.slice(0, 8);
  }

  function openAssign(course: CourseResponse) {
    setAssigningCourse(course);
    setAssignTeacherId(course.assignedTeacherId ?? "");
  }

  async function handleAssignTeacher() {
    if (!assigningCourse) return;
    setError(null);
    try {
      await api.put(`/api/admin/courses/${assigningCourse.id}/assign-teacher`, {
        teacherId: assignTeacherId || null,
      });
      setAssigningCourse(null);
      load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to assign teacher.");
    }
  }

  const canCreate = hasPermission("courses.create");
  const canUpdate = hasPermission("courses.update");
  const canDelete = hasPermission("courses.delete");

  return (
    <div>
      <PageHeader
        title="Courses"
        subtitle="Manage the course catalog"
        action={
          canCreate && (
            <Button onClick={openCreate}>+ New course</Button>
          )
        }
      />
      <ErrorBanner message={error} />
      <div className="mb-4">
        <Input
          placeholder="Search by name or code…"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          onKeyDown={(e) => e.key === "Enter" && handleSearchEnter()}
          className="max-w-xs"
        />
      </div>
      <Card>
        {isLoading ? (
          <Spinner />
        ) : !data || data.items.length === 0 ? (
          <EmptyState message="No courses yet." />
        ) : (
          <table className="w-full text-left text-sm">
            <thead className="border-b border-slate-100 text-xs uppercase tracking-wide text-slate-400">
              <tr>
                <th className="px-5 py-3">Name</th>
                <th className="px-5 py-3">Code</th>
                <th className="px-5 py-3">Duration</th>
                <th className="px-5 py-3">Teacher</th>
                <th className="px-5 py-3">Status</th>
                <th className="px-5 py-3 text-right">Actions</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map((c) => (
                <tr key={c.id} className="border-b border-slate-50 last:border-0">
                  <td className="px-5 py-3 font-medium text-ink">{c.name}</td>
                  <td className="px-5 py-3 text-slate-500">{c.code}</td>
                  <td className="px-5 py-3 text-slate-500">{c.durationInMonths} mo</td>
                  <td className="px-5 py-3 text-slate-500">{teacherName(c.assignedTeacherId) ?? "— unassigned —"}</td>
                  <td className="px-5 py-3">
                    <div className="flex gap-1.5">
                      <Badge tone={c.isActive ? "green" : "slate"}>{c.isActive ? "Active" : "Inactive"}</Badge>
                      <Badge tone={c.isPublic ? "blue" : "slate"}>{c.isPublic ? "Public" : "Private"}</Badge>
                    </div>
                  </td>
                  <td className="px-5 py-3">
                    <div className="flex justify-end gap-2">
                      {canUpdate && (
                        <>
                          <button onClick={() => openEdit(c)} className="text-xs font-medium text-brand-600 hover:underline">
                            Edit
                          </button>
                          <button onClick={() => openAssign(c)} className="text-xs font-medium text-brand-600 hover:underline">
                            Assign teacher
                          </button>
                          <button
                            onClick={() =>
                              runAction(
                                (id) => (c.isPublic ? api.post(`/api/admin/courses/${id}/unpublish`) : api.post(`/api/admin/courses/${id}/publish`)),
                                c.id,
                              )
                            }
                            className="text-xs font-medium text-slate-500 hover:underline"
                          >
                            {c.isPublic ? "Unpublish" : "Publish"}
                          </button>
                          <button
                            onClick={() =>
                              runAction(
                                (id) => (c.isActive ? api.post(`/api/admin/courses/${id}/deactivate`) : api.post(`/api/admin/courses/${id}/activate`)),
                                c.id,
                              )
                            }
                            className="text-xs font-medium text-slate-500 hover:underline"
                          >
                            {c.isActive ? "Deactivate" : "Activate"}
                          </button>
                        </>
                      )}
                      {canDelete && (
                        <button
                          onClick={() => runAction((id) => api.del(`/api/admin/courses/${id}`), c.id)}
                          className="text-xs font-medium text-rose-600 hover:underline"
                        >
                          Delete
                        </button>
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
        <Modal title={editing ? "Edit course" : "New course"} onClose={() => setModalOpen(false)}>
          <div className="space-y-3">
            <Field label="Name">
              <Input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
            </Field>
            <Field label="Code">
              <Input value={form.code} onChange={(e) => setForm({ ...form, code: e.target.value })} />
            </Field>
            <Field label="Duration (months)">
              <Input
                type="number"
                min={1}
                value={form.durationInMonths}
                onChange={(e) => setForm({ ...form, durationInMonths: Number(e.target.value) })}
              />
            </Field>
            <Field label="Description">
              <Textarea rows={3} value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
            </Field>
            {editing && (
              <Field label="Thumbnail URL">
                <Input
                  value={form.thumbnailUrl}
                  onChange={(e) => setForm({ ...form, thumbnailUrl: e.target.value })}
                  placeholder="https://cdn.example.com/courses/thumbnail.png"
                />
              </Field>
            )}
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="secondary" onClick={() => setModalOpen(false)}>
                Cancel
              </Button>
              <Button onClick={handleSubmit}>{editing ? "Save changes" : "Create course"}</Button>
            </div>
          </div>
        </Modal>
      )}

      {assigningCourse && (
        <Modal title={`Assign teacher — ${assigningCourse.name}`} onClose={() => setAssigningCourse(null)}>
          <div className="space-y-3">
            <Field label="Teacher">
              <select
                value={assignTeacherId}
                onChange={(e) => setAssignTeacherId(e.target.value)}
                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-100"
              >
                <option value="">— Unassigned —</option>
                {teachers.map((t) => (
                  <option key={t.id} value={t.id}>
                    {t.firstName} {t.lastName} {!t.isActive && "(inactive)"}
                  </option>
                ))}
              </select>
            </Field>
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="secondary" onClick={() => setAssigningCourse(null)}>
                Cancel
              </Button>
              <Button onClick={handleAssignTeacher}>Save</Button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
}
