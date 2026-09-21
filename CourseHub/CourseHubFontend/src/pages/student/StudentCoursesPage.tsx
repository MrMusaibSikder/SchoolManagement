import { useEffect, useState } from "react";
import { api } from "../../lib/api";
import { Card, EmptyState, PageHeader, Spinner } from "../../components/ui";
import type { PublicCourseResponse } from "../../lib/types";

export default function StudentCoursesPage() {
  const [courses, setCourses] = useState<PublicCourseResponse[] | null>(null);

  useEffect(() => {
    api.get<PublicCourseResponse[]>("/api/public/courses").then(setCourses);
  }, []);

  return (
    <div>
      <PageHeader title="Course catalog" subtitle="Courses currently open to the public" />
      {!courses ? (
        <Spinner />
      ) : courses.length === 0 ? (
        <EmptyState message="No public courses right now." />
      ) : (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {courses.map((c) => (
            <Card key={c.id} className="overflow-hidden">
              {c.thumbnailUrl && <img src={c.thumbnailUrl} alt={c.name} className="h-32 w-full object-cover" />}
              <div className="p-4">
                <p className="font-display text-sm font-semibold text-ink">{c.name}</p>
                <p className="mt-0.5 text-xs text-slate-400">
                  {c.code} · {c.durationInMonths} months
                </p>
                {c.description && <p className="mt-2 text-sm text-slate-500 line-clamp-3">{c.description}</p>}
              </div>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
