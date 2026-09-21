import { useEffect, useState } from "react";
import { api, ApiError } from "../lib/api";
import { useAuth } from "../lib/auth";
import type { BatchResponse, EnrollmentResponse, EnrollmentStatus, PagedResult, StudentResponse } from "../lib/types";
import { Badge, Button, Card, EmptyState, ErrorBanner, Field, Input, Modal, PageHeader, Spinner } from "../components/ui";

const STATUS_TONE: Record<EnrollmentStatus, "amber" | "green" | "blue" | "rose"> = {
  Pending: "amber",
  Active: "green",
  Completed: "blue",
  Cancelled: "rose",
};

export default function EnrollmentsPage() {
  const { hasPermission } = useAuth();
  const [data, setData] = useState<PagedResult<EnrollmentResponse> | null>(null);
  const [students, setStudents] = useState<StudentResponse[]>([]);
  const [batches, setBatches] = useState<BatchResponse[]>([]);
  const [statusFilter, setStatusFilter] = useState<EnrollmentStatus | "">("");
  const [batchFilter, setBatchFilter] = useState("");
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [createOpen, setCreateOpen] = useState(false);
  const [createForm, setCreateForm] = useState({ studentId: "", batchId: "" });

  useEffect(() => {
    // Teachers don't have students.view — this fails silently for them
    // and the table just shows raw student IDs instead of names.
    api
      .get<PagedResult<StudentResponse>>("/api/admin/students", { pageSize: 200 })
      .then((r) => setStudents(r.items))
      .catch(() => {});
    api
      .get<PagedResult<BatchResponse>>("/api/admin/batches", { pageSize: 200 })
      .then((r) => setBatches(r.items))
      .catch(() => {});
  }, []);

  async function load() {
    setIsLoading(true);
    try {
      const result = await api.get<PagedResult<EnrollmentResponse>>("/api/admin/enrollments", {
        status: statusFilter || undefined,
        batchId: batchFilter || undefined,
        page,
        pageSize: 10,
      });
      setData(result);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load enrollments.");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, statusFilter, batchFilter]);

  function studentName(id: string) {
    const s = students.find((x) => x.id === id);
    return s ? `${s.firstName} ${s.lastName}` : id.slice(0, 8);
  }

  function batchName(id: string) {
    return batches.find((b) => b.id === id)?.name ?? id.slice(0, 8);
  }

  async function handleCreate() {
    setError(null);
    try {
      await api.post("/api/admin/enrollments", createForm);
      setCreateOpen(false);
      setCreateForm({ studentId: "", batchId: "" });
      setPage(1);
      load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to create enrollment.");
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

  const canCreate = hasPermission("enrollments.create");
  const canUpdate = hasPermission("enrollments.update");
  const canDelete = hasPermission("enrollments.delete");

  return (
    <div>
      <PageHeader
        title="Enrollments"
        subtitle="Students enrolled into batches"
        action={canCreate && <Button onClick={() => setCreateOpen(true)}>+ New enrollment</Button>}
      />
      <ErrorBanner message={error} />
      <div className="mb-4 flex flex-wrap gap-3">
        <select
          value={statusFilter}
          onChange={(e) => {
            setStatusFilter(e.target.value as EnrollmentStatus | "");
            setPage(1);
          }}
          className="rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-100"
        >
          <option value="">All statuses</option>
          <option value="Pending">Pending</option>
          <option value="Active">Active</option>
          <option value="Completed">Completed</option>
          <option value="Cancelled">Cancelled</option>
        </select>
        <select
          value={batchFilter}
          onChange={(e) => {
            setBatchFilter(e.target.value);
            setPage(1);
          }}
          className="rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-100"
        >
          <option value="">All batches</option>
          {batches.map((b) => (
            <option key={b.id} value={b.id}>
              {b.name}
            </option>
          ))}
        </select>
      </div>
      <Card>
        {isLoading ? (
          <Spinner />
        ) : !data || data.items.length === 0 ? (
          <EmptyState message="No enrollments match these filters." />
        ) : (
          <table className="w-full text-left text-sm">
            <thead className="border-b border-slate-100 text-xs uppercase tracking-wide text-slate-400">
              <tr>
                <th className="px-5 py-3">Student</th>
                <th className="px-5 py-3">Batch</th>
                <th className="px-5 py-3">Enrolled on</th>
                <th className="px-5 py-3">Status</th>
                <th className="px-5 py-3 text-right">Actions</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map((en) => (
                <tr key={en.id} className="border-b border-slate-50 last:border-0">
                  <td className="px-5 py-3 font-medium text-ink">{studentName(en.studentId)}</td>
                  <td className="px-5 py-3 text-slate-500">{batchName(en.batchId)}</td>
                  <td className="px-5 py-3 text-slate-500">{new Date(en.enrollmentDate).toLocaleDateString()}</td>
                  <td className="px-5 py-3">
                    <Badge tone={STATUS_TONE[en.status]}>{en.status}</Badge>
                  </td>
                  <td className="px-5 py-3">
                    <div className="flex justify-end gap-2">
                      {canUpdate && en.status === "Pending" && (
                        <button
                          onClick={() => runAction((id) => api.post(`/api/admin/enrollments/${id}/approve`), en.id)}
                          className="text-xs font-medium text-emerald-600 hover:underline"
                        >
                          Approve
                        </button>
                      )}
                      {canUpdate && en.status === "Active" && (
                        <button
                          onClick={() => runAction((id) => api.post(`/api/admin/enrollments/${id}/complete`), en.id)}
                          className="text-xs font-medium text-blue-600 hover:underline"
                        >
                          Complete
                        </button>
                      )}
                      {canUpdate && (en.status === "Pending" || en.status === "Active") && (
                        <button
                          onClick={() => runAction((id) => api.post(`/api/admin/enrollments/${id}/cancel`), en.id)}
                          className="text-xs font-medium text-slate-500 hover:underline"
                        >
                          Cancel
                        </button>
                      )}
                      {canDelete && (en.status === "Cancelled" || en.status === "Completed") && (
                        <button
                          onClick={() => runAction((id) => api.del(`/api/admin/enrollments/${id}`), en.id)}
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

      {createOpen && (
        <Modal title="New enrollment" onClose={() => setCreateOpen(false)}>
          <div className="space-y-3">
            <Field label="Student">
              <select
                value={createForm.studentId}
                onChange={(e) => setCreateForm({ ...createForm, studentId: e.target.value })}
                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-100"
              >
                <option value="">Select a student…</option>
                {students.map((s) => (
                  <option key={s.id} value={s.id}>
                    {s.firstName} {s.lastName} ({s.studentId})
                  </option>
                ))}
              </select>
            </Field>
            <Field label="Batch">
              <select
                value={createForm.batchId}
                onChange={(e) => setCreateForm({ ...createForm, batchId: e.target.value })}
                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-100"
              >
                <option value="">Select a batch…</option>
                {batches.map((b) => (
                  <option key={b.id} value={b.id}>
                    {b.name} {!b.isActive && "(inactive)"}
                  </option>
                ))}
              </select>
            </Field>
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="secondary" onClick={() => setCreateOpen(false)}>
                Cancel
              </Button>
              <Button onClick={handleCreate}>Create enrollment</Button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
}
