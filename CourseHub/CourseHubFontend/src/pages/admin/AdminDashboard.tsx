import { useEffect, useState } from "react";
import { api } from "../../lib/api";
import { useAuth } from "../../lib/auth";
import { Card, PageHeader, Spinner } from "../../components/ui";
import type { InstitutionStatsResponse } from "../../lib/types";

export default function AdminDashboard() {
  const { currentUser } = useAuth();
  const [stats, setStats] = useState<InstitutionStatsResponse | null>(null);

  useEffect(() => {
    api.get<InstitutionStatsResponse>("/api/public/stats").then(setStats);
  }, []);

  return (
    <div>
      <PageHeader title={`Welcome back, ${currentUser?.firstName}`} subtitle="Institute-wide overview" />
      {!stats ? (
        <Spinner />
      ) : (
        <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-5">
          <StatCard label="Teachers" value={stats.totalTeachers} />
          <StatCard label="Students" value={stats.totalStudents} />
          <StatCard label="Courses" value={stats.totalCourses} />
          <StatCard label="Active batches" value={stats.totalActiveBatches} />
          <StatCard label="Enrollments" value={stats.totalEnrollments} />
        </div>
      )}
      <Card className="mt-6 p-5">
        <h2 className="font-display text-sm font-semibold text-ink">Your permissions</h2>
        <p className="mt-1 text-sm text-slate-500">
          These control which of the pages in the sidebar you can see and act on.
        </p>
        <div className="mt-3 flex flex-wrap gap-1.5">
          {currentUser?.roles.includes("SuperAdmin") ? (
            <span className="rounded-full bg-brand-50 px-2.5 py-0.5 text-xs font-medium text-brand-700">
              SuperAdmin — all permissions
            </span>
          ) : (
            currentUser?.permissions.map((p) => (
              <span key={p} className="rounded-full bg-slate-100 px-2.5 py-0.5 text-xs font-medium text-slate-600">
                {p}
              </span>
            ))
          )}
        </div>
      </Card>
    </div>
  );
}

function StatCard({ label, value }: { label: string; value: number }) {
  return (
    <Card className="p-4">
      <p className="text-xs font-medium uppercase tracking-wide text-slate-400">{label}</p>
      <p className="mt-1 font-display text-2xl font-bold text-ink">{value}</p>
    </Card>
  );
}
