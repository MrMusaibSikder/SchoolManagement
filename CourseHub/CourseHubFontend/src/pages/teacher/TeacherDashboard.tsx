import { useEffect, useState } from "react";
import { api } from "../../lib/api";
import { useAuth } from "../../lib/auth";
import { Card, PageHeader, Spinner } from "../../components/ui";
import type { InstitutionStatsResponse } from "../../lib/types";

export default function TeacherDashboard() {
  const { currentUser } = useAuth();
  const [stats, setStats] = useState<InstitutionStatsResponse | null>(null);

  useEffect(() => {
    api.get<InstitutionStatsResponse>("/api/public/stats").then(setStats);
  }, []);

  return (
    <div>
      <PageHeader title={`Welcome back, ${currentUser?.firstName}`} subtitle="Institute overview" />
      {!stats ? (
        <Spinner />
      ) : (
        <div className="grid grid-cols-2 gap-4 sm:grid-cols-3">
          <Card className="p-4">
            <p className="text-xs font-medium uppercase tracking-wide text-slate-400">Courses</p>
            <p className="mt-1 font-display text-2xl font-bold text-ink">{stats.totalCourses}</p>
          </Card>
          <Card className="p-4">
            <p className="text-xs font-medium uppercase tracking-wide text-slate-400">Active batches</p>
            <p className="mt-1 font-display text-2xl font-bold text-ink">{stats.totalActiveBatches}</p>
          </Card>
          <Card className="p-4">
            <p className="text-xs font-medium uppercase tracking-wide text-slate-400">Total enrollments</p>
            <p className="mt-1 font-display text-2xl font-bold text-ink">{stats.totalEnrollments}</p>
          </Card>
        </div>
      )}
      <Card className="mt-6 p-5">
        <h2 className="font-display text-sm font-semibold text-ink">What you can do here</h2>
        <ul className="mt-2 list-inside list-disc space-y-1 text-sm text-slate-500">
          <li>Browse the course catalog and batch schedules</li>
          <li>Review enrollment requests and approve, complete, or cancel them</li>
        </ul>
      </Card>
    </div>
  );
}
