import { Link } from "react-router-dom";
import { useSchoolInfo } from "../hooks/useSchoolInfo";

export function HeroSection() {
  const { data: school, isLoading } = useSchoolInfo();

  return (
    <header className="relative overflow-hidden bg-ink text-paper">
      {/* Faint ruled-exercise-book texture — quiet nod to a school context, not decoration for its own sake */}
      <div
        aria-hidden="true"
        className="pointer-events-none absolute inset-0 opacity-[0.06]"
        style={{
          backgroundImage:
            "repeating-linear-gradient(0deg, currentColor 0, currentColor 1px, transparent 1px, transparent 34px)",
        }}
      />

      <div className="relative mx-auto flex max-w-5xl flex-col items-start px-6 py-20 sm:px-10 sm:py-28">
        <span className="animate-fade-up mb-6 inline-flex items-center gap-2 rounded-full border border-gold/40 px-3 py-1 text-xs font-medium uppercase tracking-[0.2em] text-gold [animation-delay:0ms]">
          School Portal
        </span>

        <h1 className="animate-fade-up max-w-3xl font-display text-4xl font-semibold leading-[1.05] tracking-tight sm:text-6xl [animation-delay:80ms]">
          {isLoading ? (
            <span className="inline-block h-[1.05em] w-[10ch] animate-pulse rounded bg-paper/10 align-bottom" />
          ) : (
            (school?.name ?? "School Management System")
          )}
        </h1>

        <div className="animate-fade-up mt-5 h-px w-24 bg-gold [animation-delay:160ms]" />

        <p className="animate-fade-up mt-6 max-w-xl text-base leading-relaxed text-paper/70 sm:text-lg [animation-delay:220ms]">
          {school?.address ??
            "Academic records, attendance, results, and fee information — kept in one place, for the whole school."}
        </p>

        <div className="animate-fade-up mt-10 flex flex-wrap items-center gap-4 [animation-delay:300ms]">
          <Link
            to="/login"
            className="inline-flex items-center justify-center rounded-md border border-gold bg-gold px-6 py-3 text-sm font-semibold text-ink transition hover:bg-gold/90 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-gold"
          >
            Student &amp; Staff Login
          </Link>
          <a
            href="#notice-board"
            className="inline-flex items-center justify-center rounded-md border border-paper/25 px-6 py-3 text-sm font-medium text-paper/90 transition hover:border-paper/50 hover:bg-paper/5 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-paper/60"
          >
            View Notices
          </a>
        </div>
      </div>
    </header>
  );
}
