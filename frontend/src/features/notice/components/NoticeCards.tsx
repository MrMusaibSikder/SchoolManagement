import { CalendarDays, Megaphone } from "lucide-react";
import type { ReactNode } from "react";
import type { NoticeDto, NoticePriority } from "../types/notice.types";

const priorityStyles: Record<
  NoticePriority,
  { label: string; card: string; icon: string; badge: string }
> = {
  1: {
    label: "Low priority",
    card: "border-emerald-200 bg-gradient-to-br from-emerald-50 to-white",
    icon: "bg-emerald-100 text-emerald-700",
    badge: "bg-emerald-100 text-emerald-800",
  },
  2: {
    label: "Medium priority",
    card: "border-amber-200 bg-gradient-to-br from-amber-50 to-white",
    icon: "bg-amber-100 text-amber-700",
    badge: "bg-amber-100 text-amber-800",
  },
  3: {
    label: "High priority",
    card: "border-rose-200 bg-gradient-to-br from-rose-50 to-white",
    icon: "bg-rose-100 text-rose-700",
    badge: "bg-rose-100 text-rose-800",
  },
};

function formatDate(value: string): string {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "Date unavailable";

  return new Intl.DateTimeFormat("en-BD", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  }).format(date);
}

export function NoticeCards({
  notices,
  layout = "grid",
  showStatus = false,
  renderActions,
}: {
  notices: NoticeDto[];
  layout?: "grid" | "list";
  showStatus?: boolean;
  renderActions?: (notice: NoticeDto) => ReactNode;
}) {
  return (
    <ul
      className={
        layout === "grid"
          ? "grid gap-4 sm:grid-cols-2 xl:grid-cols-3"
          : "space-y-3"
      }
    >
      {notices.map((notice) => {
        const styles = priorityStyles[notice.priority] ?? priorityStyles[2];
        const description = notice.description.trim();
        const displayedDescription =
          layout === "list" && description.length > 120
            ? `${description.slice(0, 120)}…`
            : description;

        return (
          <li
            key={notice.id}
            className={`overflow-hidden rounded-xl border p-4 shadow-sm transition-shadow hover:shadow-md ${styles.card}`}
          >
            <div className="flex items-start gap-3">
              <div
                aria-hidden="true"
                className={`flex h-10 w-10 shrink-0 items-center justify-center rounded-xl ${styles.icon}`}
              >
                <Megaphone className="h-5 w-5" />
              </div>
              <div className="min-w-0 flex-1">
                <div className="flex flex-wrap items-center justify-between gap-2">
                  <span
                    className={`rounded-full px-2.5 py-1 text-[11px] font-semibold ${styles.badge}`}
                  >
                    {styles.label}
                  </span>
                  {showStatus ? (
                    <span className="rounded-full bg-background/80 px-2.5 py-1 text-[11px] font-semibold text-muted-foreground">
                      {notice.isArchived
                        ? "Archived"
                        : notice.isPublished
                          ? "Published"
                          : "Draft"}
                    </span>
                  ) : null}
                  {notice.isUpcoming ? (
                    <span className="rounded-full bg-sky-100 px-2.5 py-1 text-[11px] font-semibold text-sky-800">
                      Upcoming
                    </span>
                  ) : null}
                </div>
                <h3 className="mt-3 break-words font-semibold leading-snug text-foreground">
                  {notice.title}
                </h3>
              </div>
            </div>

            {displayedDescription ? (
              <p className="mt-3 whitespace-pre-wrap break-words text-sm leading-relaxed text-muted-foreground">
                {displayedDescription}
              </p>
            ) : null}

            <div className="mt-4 flex flex-wrap items-center gap-x-4 gap-y-2 border-t border-current/10 pt-3 text-xs text-muted-foreground">
              {showStatus ? (
                <span>
                  Audience:{" "}
                  {["Unknown", "Students", "Teachers", "Employees", "Guardians", "Everyone"][
                    notice.audience
                  ] ?? "Unknown"}
                </span>
              ) : null}
              <span className="inline-flex items-center gap-1.5">
                <CalendarDays aria-hidden="true" className="h-3.5 w-3.5" />
                Published {formatDate(notice.publishDate)}
              </span>
              {notice.expiryDate ? (
                <span>Expires {formatDate(notice.expiryDate)}</span>
              ) : null}
            </div>
            {renderActions ? (
              <div className="mt-3 flex justify-end">{renderActions(notice)}</div>
            ) : null}
          </li>
        );
      })}
    </ul>
  );
}
