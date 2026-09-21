import { useEffect, useState } from "react";
import { api, ApiError } from "../../lib/api";
import { useAuth } from "../../lib/auth";
import type { BatchResponse, CourseResponse, PagedResult } from "../../lib/types";
import { Badge, Button, Card, EmptyState, ErrorBanner, Field, Input, Modal, PageHeader, Spinner } from "../../components/ui";

const emptyForm = { courseId: "", name: "", code: "", startDate: "", capacity: "" };

export default function BatchesPage() {
  const { hasPermission } = useAuth();
  const [data, setData] = useState<PagedResult<BatchResponse> | null>(null);
  const [courses, setCourses] = useState<CourseResponse[]>([]);
  const [courseFilter, setCourseFilter] = useState("");
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState(emptyForm);

  async function load(pageOverride?: number) {
    setIsLoading(true);
    try {
      const result = await api.get<PagedResult<BatchResponse>>("/api/admin/batches", {
        courseId: courseFilter || undefined,
        page: pageOverride ?? page,
        pageSize: 10,
      });
      setData(result);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load batches.");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    api
      .get<PagedResult<CourseResponse>>("/api/admin/courses", { pageSize: 100 })
      .then((r) => setCourses(r.items))
      .catch(() => {});
  }, []);

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, courseFilter]);

  function courseName(id: string) {
    return courses.find((c) => c.id === id)?.name ?? id.slice(0, 8);
  }

  function openCreate() {
    setForm({ ...emptyForm, courseId: courses[0]?.id ?? "" });
    setModalOpen(true);
  }

  async function handleSubmit() {
    setError(null);
    try {
      await api.post("/api/admin/batches", {
        courseId: form.courseId,
        name: form.name,
        code: form.code,
        startDate: form.startDate,
        capacity: form.capacity ? Number(form.capacity) : null,
      });
      setModalOpen(false);
      setPage(1);
      load(1);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to save batch.");
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

  const canCreate = hasPermission("batches.create");
  const canUpdate = hasPermission("batches.update");
  const canDelete = hasPermission("batches.delete");

  return (
    <div>
      <PageHeader
        title="Batches"
        subtitle="Cohorts scheduled under a course"
        action={canCreate && courses.length > 0 && <Button onClick={openCreate}>+ New batch</Button>}
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
          <EmptyState message="No batches yet." />
        ) : (
          <table className="w-full text-left text-sm">
            <thead className="border-b border-slate-100 text-xs uppercase tracking-wide text-slate-400">
              <tr>
                <th className="px-5 py-3">Batch</th>
                <th className="px-5 py-3">Course</th>
                <th className="px-5 py-3">Start date</th>
                <th className="px-5 py-3">Capacity</th>
                <th className="px-5 py-3">Status</th>
                <th className="px-5 py-3 text-right">Actions</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map((b) => (
                <tr key={b.id} className="border-b border-slate-50 last:border-0">
                  <td className="px-5 py-3 font-medium text-ink">
                    {b.name} <span className="text-slate-400">({b.code})</span>
                  </td>
                  <td className="px-5 py-3 text-slate-500">{courseName(b.courseId)}</td>
                  <td className="px-5 py-3 text-slate-500">{new Date(b.startDate).toLocaleDateString()}</td>
                  <td className="px-5 py-3 text-slate-500">{b.capacity ?? "Unlimited"}</td>
                  <td className="px-5 py-3">
                    <Badge tone={b.isActive ? "green" : "slate"}>{b.isActive ? "Active" : "Inactive"}</Badge>
                  </td>
                  <td className="px-5 py-3">
                    <div className="flex justify-end gap-2">
                      {canUpdate && (
                        <button
                          onClick={() =>
                            runAction(
                              (id) => (b.isActive ? api.post(`/api/admin/batches/${id}/deactivate`) : api.post(`/api/admin/batches/${id}/activate`)),
                              b.id,
                            )
                          }
                          className="text-xs font-medium text-slate-500 hover:underline"
                        >
                          {b.isActive ? "Deactivate" : "Activate"}
                        </button>
                      )}
                      {canDelete && (
                        <button
                          onClick={() => runAction((id) => api.del(`/api/admin/batches/${id}`), b.id)}
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
        <Modal title="New batch" onClose={() => setModalOpen(false)}>
          <div className="space-y-3">
            <Field label="Course">
              <select
                value={form.courseId}
                onChange={(e) => setForm({ ...form, courseId: e.target.value })}
                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-100"
              >
                {courses.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name} {!c.isActive && "(inactive)"}
                  </option>
                ))}
              </select>
            </Field>
            <Field label="Batch name">
              <Input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} placeholder="e.g. Morning Batch — Jan 2026" />
            </Field>
            <Field label="Code">
              <Input value={form.code} onChange={(e) => setForm({ ...form, code: e.target.value })} />
            </Field>
            <Field label="Start date">
              <Input type="date" value={form.startDate} onChange={(e) => setForm({ ...form, startDate: e.target.value })} />
            </Field>
            <Field label="Capacity (optional)">
              <Input type="number" min={1} value={form.capacity} onChange={(e) => setForm({ ...form, capacity: e.target.value })} placeholder="Leave blank for unlimited" />
            </Field>
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="secondary" onClick={() => setModalOpen(false)}>
                Cancel
              </Button>
              <Button onClick={handleSubmit}>Create batch</Button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
}
