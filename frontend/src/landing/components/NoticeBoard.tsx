import { usePublicNotices } from "../hooks/usePublicNotices";
import type { PublicNoticeDto } from "../types/public.types";

/**
 * Maps NoticePriority (sent from the backend as a plain string) to a pin
 * color. The exact enum member names haven't been confirmed against
 * NoticePriority.cs yet, so this matches loosely and case-insensitively,
 * falling back to the default gold pin for anything unrecognized — an
 * unexpected value should never break rendering. Tighten this once the
 * real enum is confirmed.
 */
function pinColorFor(priority: string): string {
  const key = priority.trim().toLowerCase();
  if (key.includes("urgent") || key.includes("high")) return "bg-rust";
  if (key.includes("low")) return "bg-forest";
  return "bg-gold"; // medium / default / unrecognized
}

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString("en-GB", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  });
}

function NoticeCard({
  notice,
  index,
}: {
  notice: PublicNoticeDto;
  index: number;
}) {
  const tilt = index % 2 === 0 ? "-rotate-1" : "rotate-1";

  return (
    <li
      className={`relative rounded-sm border border-ink/10 bg-paper p-5 shadow-[0_10px_24px_-14px_rgba(15,42,61,0.35)] transition-transform duration-200 hover:-translate-y-0.5 hover:rotate-0 motion-reduce:transform-none ${tilt}`}
    >
      <span
        aria-hidden="true"
        className={`absolute left-1/2 top-0 h-3 w-3 -translate-x-1/2 -translate-y-1/2 rounded-full ring-2 ring-paper ${pinColorFor(
          notice.priority
        )}`}
      />
      <time
        dateTime={notice.publishDate}
        className="font-mono text-[11px] uppercase tracking-wide text-ink-muted"
      >
        {formatDate(notice.publishDate)}
      </time>
      <h3 className="mt-2 font-display text-lg font-semibold leading-snug text-ink">
        {notice.title}
      </h3>
      {notice.summary && (
        <p className="mt-2 text-sm leading-relaxed text-ink-muted">
          {notice.summary}
        </p>
      )}
    </li>
  );
}

function NoticeBoardSkeleton() {
  return (
    <ul className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3" aria-hidden="true">
      {Array.from({ length: 3 }).map((_, i) => (
        <li
          key={i}
          className="h-40 animate-pulse rounded-sm border border-ink/10 bg-paper/70"
        />
      ))}
    </ul>
  );
}

export function NoticeBoard() {
  const { data: notices, isLoading, isError } = usePublicNotices(6);

  return (
    <section id="notice-board" className="bg-cork px-6 py-20 sm:px-10">
      <div className="mx-auto max-w-5xl">
        <div className="mb-10">
          <span className="text-xs font-medium uppercase tracking-[0.2em] text-forest">
            Announcements
          </span>
          <h2 className="mt-2 font-display text-3xl font-semibold text-ink">
            Notice Board
          </h2>
        </div>

        {isLoading && <NoticeBoardSkeleton />}

        {isError && (
          <p className="rounded-sm border border-rust/30 bg-paper px-5 py-4 text-sm text-rust">
            Notices could not be loaded right now. Please check back shortly.
          </p>
        )}

        {!isLoading && !isError && notices && notices.length === 0 && (
          <p className="rounded-sm border border-ink/10 bg-paper px-5 py-8 text-center text-sm text-ink-muted">
            No public notices at the moment.
          </p>
        )}

        {!isLoading && !isError && notices && notices.length > 0 && (
          <ul className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {notices.map((notice, i) => (
              <NoticeCard key={notice.id} notice={notice} index={i} />
            ))}
          </ul>
        )}
      </div>
    </section>
  );
}
