import { useEffect, useState } from "react";
import { api, ApiError } from "../../lib/api";
import { useAuth } from "../../lib/auth";
import type { EnrollmentResponse, EnrollmentStatus, PagedResult } from "../../lib/types";
import { Badge, Card, EmptyState, ErrorBanner, PageHeader, Spinner } from "../../components/ui";

const STATUS_TONE: Record<EnrollmentStatus, "amber" | "green" | "blue" | "rose"> = {
  Pending: "amber",
  Active: "green",
  Completed: "blue",
  Cancelled: "rose",
};

export default function StudentDashboard() {
  const { currentUser } = useAuth();
  const [enrollments, setEnrollments] = useState<PagedResult<EnrollmentResponse> | null>(null);
  const [hasProfile, setHasProfile] = useState(true);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    api
      .get<PagedResult<EnrollmentResponse>>("/api/me/enrollments", { pageSize: 50 })
      .then(setEnrollments)
      .catch((err) => {
        // 404 here specifically means "no student profile yet" (see the
        // backend's MeService) — not a real error, just an expected
        // state for a Student-role account an admin hasn't promoted yet.
        if (err instanceof ApiError && err.status === 404) {
          setHasProfile(false);
        } else {
          setError(err instanceof ApiError ? err.message : "Failed to load your enrollments.");
        }
      })
      .finally(() => setIsLoading(false));
  }, []);

  return (
    <div>
      <PageHeader title={`Hi, ${currentUser?.firstName}!`} subtitle="Your CourseHub account" />
      <Card className="p-5">
        <h2 className="font-display text-sm font-semibold text-ink">Your account</h2>
        <dl className="mt-3 grid grid-cols-2 gap-3 text-sm">
          <div>
            <dt className="text-xs font-medium uppercase tracking-wide text-slate-400">Name</dt>
            <dd className="mt-0.5 text-ink">
              {currentUser?.firstName} {currentUser?.lastName}
            </dd>
          </div>
          <div>
            <dt className="text-xs font-medium uppercase tracking-wide text-slate-400">Email</dt>
            <dd className="mt-0.5 text-ink">{currentUser?.email}</dd>
          </div>
        </dl>
      </Card>

      <Card className="mt-6 p-5">
        <h2 className="font-display text-sm font-semibold text-ink">Your enrollments</h2>
        <ErrorBanner message={error} />
        {isLoading ? (
          <Spinner />
        ) : !hasProfile ? (
          <p className="mt-2 rounded-lg bg-amber-50 px-4 py-3 text-sm text-amber-700">
            You don't have a student profile yet — ask an administrator to create one for you before you can be
            enrolled in a batch.
          </p>
        ) : !enrollments || enrollments.items.length === 0 ? (
          <EmptyState message="You're not enrolled in anything yet. Browse the course catalog, then ask an admin to enroll you." />
        ) : (
          <table className="mt-3 w-full text-left text-sm">
            <thead className="border-b border-slate-100 text-xs uppercase tracking-wide text-slate-400">
              <tr>
                <th className="py-2 pr-4">Enrolled on</th>
                <th className="py-2 pr-4">Batch</th>
                <th className="py-2 pr-4">Status</th>
              </tr>
            </thead>
            <tbody>
              {enrollments.items.map((e) => (
                <tr key={e.id} className="border-b border-slate-50 last:border-0">
                  <td className="py-2 pr-4 text-slate-500">{new Date(e.enrollmentDate).toLocaleDateString()}</td>
                  {/* Only the raw batch id, not its name — a Student
                      account has no permission to look up batch/course
                      details (that's admin/teacher-only data), so there's
                      no name to resolve it to on this dashboard. */}
                  <td className="py-2 pr-4 font-mono text-xs text-slate-400">{e.batchId.slice(0, 8)}…</td>
                  <td className="py-2 pr-4">
                    <Badge tone={STATUS_TONE[e.status]}>{e.status}</Badge>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </Card>
    </div>
  );
}
