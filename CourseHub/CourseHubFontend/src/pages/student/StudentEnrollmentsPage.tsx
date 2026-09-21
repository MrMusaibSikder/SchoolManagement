import { useEffect, useState } from "react";
import { api, ApiError } from "../../lib/api";
import type { BatchResponse, EnrollmentResponse, EnrollmentStatus, PagedResult, PublicCourseResponse } from "../../lib/types";
import { Badge, Button, Card, EmptyState, ErrorBanner, Field, Modal, PageHeader, Spinner } from "../../components/ui";

const STATUS_TONE: Record<EnrollmentStatus, "amber" | "green" | "blue" | "rose"> = {
  Pending: "amber",
  Active: "green",
  Completed: "blue",
  Cancelled: "rose",
};

export default function StudentEnrollmentsPage() {
  const [data, setData] = useState<PagedResult<EnrollmentResponse> | null>(null);
  const [courses, setCourses] = useState<PublicCourseResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [requestOpen, setRequestOpen] = useState(false);
  const [selectedCourseId, setSelectedCourseId] = useState("");
  const [batches, setBatches] = useState<BatchResponse[] | null>(null);
  const [batchesLoading, setBatchesLoading] = useState(false);
  const [selectedBatchId, setSelectedBatchId] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function load() {
    setIsLoading(true);
    try {
      const result = await api.get<PagedResult<EnrollmentResponse>>("/api/me/enrollments", { pageSize: 50 });
      setData(result);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load your enrollments.");
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
    return courses.find((c) => c.id === id)?.name ?? id.slice(0, 8);
  }

  function batchName(id: string) {
    return batches?.find((b) => b.id === id)?.name ?? id.slice(0, 8);
  }

  function openRequest() {
    setRequestOpen(true);
    setSelectedCourseId("");
    setBatches(null);
    setSelectedBatchId("");
    setError(null);
  }

  async function onCourseChange(courseId: string) {
    setSelectedCourseId(courseId);
    setSelectedBatchId("");
    setBatches(null);
    if (!courseId) return;
    setBatchesLoading(true);
    try {
      const result = await api.get<BatchResponse[]>("/api/me/batches", { courseId });
      setBatches(result);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load batches for this course.");
    } finally {
      setBatchesLoading(false);
    }
  }

  async function handleRequest() {
    if (!selectedBatchId) return;
    setIsSubmitting(true);
    setError(null);
    try {
      await api.post("/api/me/enrollments", { batchId: selectedBatchId });
      setRequestOpen(false);
      load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to request enrollment.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div>
      <PageHeader
        title="My enrollments"
        subtitle="Batches you're enrolled in, or have asked to join"
        action={<Button onClick={openRequest}>+ Request enrollment</Button>}
      />
      <ErrorBanner message={error} />
      <Card>
        {isLoading ? (
          <Spinner />
        ) : !data || data.items.length === 0 ? (
          <EmptyState message="You haven't requested any enrollments yet." />
        ) : (
          <table className="w-full text-left text-sm">
            <thead className="border-b border-slate-100 text-xs uppercase tracking-wide text-slate-400">
              <tr>
                <th className="px-5 py-3">Batch</th>
                <th className="px-5 py-3">Requested on</th>
                <th className="px-5 py-3">Status</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map((en) => (
                <tr key={en.id} className="border-b border-slate-50 last:border-0">
                  <td className="px-5 py-3 font-medium text-ink">{batchName(en.batchId)}</td>
                  <td className="px-5 py-3 text-slate-500">{new Date(en.enrollmentDate).toLocaleDateString()}</td>
                  <td className="px-5 py-3">
                    <Badge tone={STATUS_TONE[en.status]}>{en.status}</Badge>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </Card>

      {requestOpen && (
        <Modal title="Request enrollment" onClose={() => setRequestOpen(false)}>
          <div className="space-y-3">
            <p className="text-xs text-slate-500">
              A teacher or admin needs to confirm your request before you're actually enrolled.
            </p>
            <Field label="Course">
              <select
                value={selectedCourseId}
                onChange={(e) => onCourseChange(e.target.value)}
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
            {selectedCourseId && (
              <Field label="Batch">
                {batchesLoading ? (
                  <Spinner />
                ) : !batches || batches.length === 0 ? (
                  <p className="text-xs text-slate-400">No active batches open for this course right now.</p>
                ) : (
                  <select
                    value={selectedBatchId}
                    onChange={(e) => setSelectedBatchId(e.target.value)}
                    className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-100"
                  >
                    <option value="">Select a batch…</option>
                    {batches.map((b) => (
                      <option key={b.id} value={b.id}>
                        {b.name} ({b.code})
                      </option>
                    ))}
                  </select>
                )}
              </Field>
            )}
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="secondary" onClick={() => setRequestOpen(false)}>
                Cancel
              </Button>
              <Button onClick={handleRequest} disabled={!selectedBatchId || isSubmitting}>
                {isSubmitting ? "Sending…" : "Send request"}
              </Button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
}
