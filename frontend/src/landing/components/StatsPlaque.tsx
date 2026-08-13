import { usePublicStats } from "../hooks/usePublicStats";
import type { PublicStatsDto } from "../types/public.types";

const STAT_ROWS: { key: keyof PublicStatsDto; label: string }[] = [
  { key: "totalStudents", label: "Students" },
  { key: "totalTeachers", label: "Teachers" },
  { key: "totalEmployees", label: "Staff" },
];

/**
 * A horizontal "plaque" of figures overlapping the hero's bottom edge —
 * styled like a mark-sheet/ledger row rather than the generic
 * icon-plus-big-number card grid. Paper-white against the navy hero above
 * reads as a brass plaque mounted on a gate.
 */
export function StatsPlaque() {
  const { data, isLoading } = usePublicStats();

  return (
    <div className="relative z-10 mx-auto -mt-10 max-w-4xl px-6 sm:-mt-14 sm:px-10">
      <dl className="grid grid-cols-3 divide-x divide-ink/10 rounded-lg border border-ink/10 bg-paper text-ink shadow-[0_18px_40px_-20px_rgba(15,42,61,0.35)]">
        {STAT_ROWS.map((row) => (
          <div
            key={row.key}
            className="flex flex-col items-center gap-1 px-3 py-6 sm:py-8"
          >
            <dd className="order-1 font-mono text-3xl font-semibold tabular-nums sm:text-4xl">
              {isLoading || !data ? (
                <span className="inline-block h-[1em] w-[3ch] animate-pulse rounded bg-ink/10 align-bottom" />
              ) : (
                data[row.key].toLocaleString()
              )}
            </dd>
            <dt className="order-2 text-[11px] font-medium uppercase tracking-[0.16em] text-ink-muted">
              {row.label}
            </dt>
          </div>
        ))}
      </dl>
    </div>
  );
}
