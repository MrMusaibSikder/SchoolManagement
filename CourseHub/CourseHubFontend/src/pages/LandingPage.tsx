import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { api } from "../lib/api";
import type {
  InstitutionProfileResponse,
  InstitutionStatsResponse,
  PublicCourseResponse,
  PublicTeacherResponse,
} from "../lib/types";

const COURSE_ACCENTS = [
  "from-brand-500 to-sky-400",
  "from-fuchsia-500 to-pink-400",
  "from-amber-500 to-orange-400",
  "from-emerald-500 to-teal-400",
  "from-violet-500 to-indigo-400",
  "from-rose-500 to-red-400",
];

const AVATAR_ACCENTS = [
  "from-brand-500 to-sky-400",
  "from-fuchsia-500 to-pink-400",
  "from-amber-500 to-orange-400",
  "from-emerald-500 to-teal-400",
  "from-violet-500 to-indigo-400",
];

function initials(first: string, last: string) {
  return `${first.charAt(0)}${last.charAt(0)}`.toUpperCase();
}

export default function LandingPage() {
  const [institution, setInstitution] = useState<InstitutionProfileResponse | null>(null);
  const [stats, setStats] = useState<InstitutionStatsResponse | null>(null);
  const [courses, setCourses] = useState<PublicCourseResponse[]>([]);
  const [teachers, setTeachers] = useState<PublicTeacherResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Every one of these is an unauthenticated /api/public/* endpoint, so
    // no login/permission is needed to render this page. Promise.allSettled
    // so one slow/missing endpoint doesn't blank out the whole page.
    Promise.allSettled([
      api.get<InstitutionProfileResponse>("/api/public/institution"),
      api.get<InstitutionStatsResponse>("/api/public/stats"),
      api.get<PublicCourseResponse[]>("/api/public/courses"),
      api.get<PublicTeacherResponse[]>("/api/public/teachers"),
    ]).then(([inst, statsRes, coursesRes, teachersRes]) => {
      if (inst.status === "fulfilled") setInstitution(inst.value);
      if (statsRes.status === "fulfilled") setStats(statsRes.value);
      if (coursesRes.status === "fulfilled") setCourses(coursesRes.value);
      if (teachersRes.status === "fulfilled") setTeachers(teachersRes.value);
      setIsLoading(false);
    });
  }, []);

  const name = institution?.name ?? "CourseHub";

  return (
    <div className="min-h-screen bg-slate-50">
      {/* Nav */}
      <header className="sticky top-0 z-40 border-b border-slate-200/70 bg-white/80 backdrop-blur">
        <div className="mx-auto flex max-w-6xl items-center justify-between px-4 py-3 sm:px-6">
          <div className="flex items-center gap-2">
            {institution?.logoUrl ? (
              <img src={institution.logoUrl} alt={name} className="h-8 w-8 rounded-lg object-cover" />
            ) : (
              <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-gradient-to-br from-brand-500 to-fuchsia-500 text-sm font-bold text-white">
                {name.charAt(0)}
              </div>
            )}
            <span className="font-display text-lg font-bold text-ink">{name}</span>
          </div>
          <div className="flex items-center gap-2">
            <Link
              to="/login"
              className="rounded-lg px-3.5 py-2 text-sm font-medium text-slate-600 transition-colors hover:bg-slate-100"
            >
              Sign in
            </Link>
            <Link
              to="/register"
              className="rounded-lg bg-gradient-to-r from-brand-500 to-fuchsia-500 px-3.5 py-2 text-sm font-medium text-white shadow-sm transition-opacity hover:opacity-90"
            >
              Get started
            </Link>
          </div>
        </div>
      </header>

      {/* Hero */}
      <section className="relative overflow-hidden bg-gradient-to-br from-brand-600 via-indigo-600 to-fuchsia-600">
        <div className="pointer-events-none absolute -left-24 -top-24 h-72 w-72 rounded-full bg-white/10 blur-3xl" />
        <div className="pointer-events-none absolute -bottom-32 -right-16 h-80 w-80 rounded-full bg-fuchsia-400/20 blur-3xl" />
        <div className="relative mx-auto max-w-6xl px-4 py-20 text-center sm:px-6 sm:py-28">
          <h1 className="font-display text-3xl font-bold text-white sm:text-5xl">
            Learn something new at <span className="text-amber-300">{name}</span>
          </h1>
          <p className="mx-auto mt-4 max-w-2xl text-base text-brand-50 sm:text-lg">
            {institution?.description ??
              "Browse our courses, meet our teachers, and join a batch — everything you need to get started is right here."}
          </p>
          <div className="mt-8 flex flex-wrap items-center justify-center gap-3">
            <Link
              to="/register"
              className="rounded-lg bg-white px-5 py-2.5 text-sm font-semibold text-brand-700 shadow-lg transition-transform hover:scale-105"
            >
              Create an account
            </Link>
            <Link
              to="/login"
              className="rounded-lg border border-white/40 bg-white/10 px-5 py-2.5 text-sm font-semibold text-white backdrop-blur transition-colors hover:bg-white/20"
            >
              Sign in
            </Link>
          </div>
        </div>
      </section>

      {/* Stats */}
      {stats && (
        <section className="mx-auto -mt-10 max-w-6xl px-4 sm:px-6">
          <div className="grid grid-cols-2 gap-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-lg sm:grid-cols-5 sm:gap-4 sm:p-6">
            {[
              { label: "Courses", value: stats.totalCourses, emoji: "📚" },
              { label: "Teachers", value: stats.totalTeachers, emoji: "🎓" },
              { label: "Students", value: stats.totalStudents, emoji: "🧑‍💻" },
              { label: "Active batches", value: stats.totalActiveBatches, emoji: "🗓️" },
              { label: "Enrollments", value: stats.totalEnrollments, emoji: "✅" },
            ].map((s) => (
              <div key={s.label} className="flex flex-col items-center rounded-xl px-2 py-3 text-center">
                <span className="text-2xl">{s.emoji}</span>
                <span className="font-display mt-1 text-2xl font-bold text-ink">{s.value}</span>
                <span className="text-xs text-slate-500">{s.label}</span>
              </div>
            ))}
          </div>
        </section>
      )}

      {isLoading && (
        <div className="flex justify-center py-16">
          <div className="h-8 w-8 animate-spin rounded-full border-4 border-slate-200 border-t-brand-500" />
        </div>
      )}

      {/* Courses */}
      {!isLoading && courses.length > 0 && (
        <section className="mx-auto max-w-6xl px-4 py-16 sm:px-6">
          <div className="mb-8 text-center">
            <h2 className="font-display text-2xl font-bold text-ink sm:text-3xl">Our courses</h2>
            <p className="mt-2 text-sm text-slate-500">Pick a course and start your journey today.</p>
          </div>
          <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-3">
            {courses.map((c, i) => (
              <div
                key={c.id}
                className="group overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm transition-shadow hover:shadow-lg"
              >
                <div className={`h-2 w-full bg-gradient-to-r ${COURSE_ACCENTS[i % COURSE_ACCENTS.length]}`} />
                {c.thumbnailUrl && (
                  <img src={c.thumbnailUrl} alt={c.name} className="h-36 w-full object-cover" />
                )}
                <div className="p-5">
                  <div className="flex items-start justify-between gap-2">
                    <h3 className="font-display text-base font-semibold text-ink">{c.name}</h3>
                    <span className="shrink-0 rounded-full bg-slate-100 px-2 py-0.5 text-xs font-medium text-slate-500">
                      {c.code}
                    </span>
                  </div>
                  {c.description && <p className="mt-2 line-clamp-2 text-sm text-slate-500">{c.description}</p>}
                  <p className="mt-3 text-xs font-medium text-brand-600">{c.durationInMonths} month course</p>
                </div>
              </div>
            ))}
          </div>
        </section>
      )}

      {/* Teachers */}
      {!isLoading && teachers.length > 0 && (
        <section className="bg-white py-16">
          <div className="mx-auto max-w-6xl px-4 sm:px-6">
            <div className="mb-8 text-center">
              <h2 className="font-display text-2xl font-bold text-ink sm:text-3xl">Meet our teachers</h2>
              <p className="mt-2 text-sm text-slate-500">Learn from people who love what they teach.</p>
            </div>
            <div className="grid grid-cols-2 gap-5 sm:grid-cols-3 lg:grid-cols-4">
              {teachers.map((t, i) => (
                <div key={t.id} className="flex flex-col items-center rounded-2xl border border-slate-100 p-5 text-center">
                  {t.profileImageUrl ? (
                    <img src={t.profileImageUrl} alt={t.firstName} className="h-16 w-16 rounded-full object-cover" />
                  ) : (
                    <div
                      className={`flex h-16 w-16 items-center justify-center rounded-full bg-gradient-to-br text-lg font-bold text-white ${AVATAR_ACCENTS[i % AVATAR_ACCENTS.length]}`}
                    >
                      {initials(t.firstName, t.lastName)}
                    </div>
                  )}
                  <p className="mt-3 text-sm font-semibold text-ink">
                    {t.firstName} {t.lastName}
                  </p>
                  {t.bio && <p className="mt-1 line-clamp-2 text-xs text-slate-500">{t.bio}</p>}
                </div>
              ))}
            </div>
          </div>
        </section>
      )}

      {/* CTA banner */}
      <section className="mx-auto max-w-6xl px-4 pb-16 sm:px-6">
        <div className="overflow-hidden rounded-2xl bg-gradient-to-r from-brand-600 to-fuchsia-600 px-6 py-10 text-center shadow-lg sm:px-12">
          <h2 className="font-display text-xl font-bold text-white sm:text-2xl">Ready to get started?</h2>
          <p className="mt-2 text-sm text-brand-50">Create your free account and request to join a batch today.</p>
          <Link
            to="/register"
            className="mt-6 inline-block rounded-lg bg-white px-5 py-2.5 text-sm font-semibold text-brand-700 shadow-lg transition-transform hover:scale-105"
          >
            Register now
          </Link>
        </div>
      </section>

      {/* Footer */}
      <footer className="border-t border-slate-200 bg-white">
        <div className="mx-auto max-w-6xl px-4 py-8 text-center text-xs text-slate-400 sm:px-6">
          {(institution?.address || institution?.phone || institution?.email) && (
            <p className="mb-2">
              {[institution?.address, institution?.phone, institution?.email].filter(Boolean).join(" · ")}
            </p>
          )}
          <p>
            © {new Date().getFullYear()} {name}. All rights reserved.
          </p>
        </div>
      </footer>
    </div>
  );
}
