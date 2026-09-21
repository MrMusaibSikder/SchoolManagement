import { useEffect, useState } from "react";
import { api } from "../../lib/api";
import { Card, EmptyState, PageHeader, Spinner } from "../../components/ui";
import type { PublicTeacherResponse } from "../../lib/types";

export default function StudentTeachersPage() {
  const [teachers, setTeachers] = useState<PublicTeacherResponse[] | null>(null);

  useEffect(() => {
    api.get<PublicTeacherResponse[]>("/api/public/teachers").then(setTeachers);
  }, []);

  return (
    <div>
      <PageHeader title="Our teachers" subtitle="Teachers who've made their profile public" />
      {!teachers ? (
        <Spinner />
      ) : teachers.length === 0 ? (
        <EmptyState message="No public teacher profiles right now." />
      ) : (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {teachers.map((t) => (
            <Card key={t.id} className="p-4">
              <div className="flex items-center gap-3">
                {t.profileImageUrl ? (
                  <img src={t.profileImageUrl} alt="" className="h-12 w-12 rounded-full object-cover" />
                ) : (
                  <div className="flex h-12 w-12 items-center justify-center rounded-full bg-brand-50 font-display text-sm font-semibold text-brand-600">
                    {t.firstName[0]}
                    {t.lastName[0]}
                  </div>
                )}
                <div>
                  <p className="font-display text-sm font-semibold text-ink">
                    {t.firstName} {t.lastName}
                  </p>
                </div>
              </div>
              {t.bio && <p className="mt-3 text-sm text-slate-500 line-clamp-3">{t.bio}</p>}
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
