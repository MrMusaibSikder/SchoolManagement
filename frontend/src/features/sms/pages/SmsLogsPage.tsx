import { useMemo, useRef, useState } from "react";
import { Link } from "react-router-dom";
import {
  AlertCircle,
  CalendarDays,
  CheckCircle2,
  Clock3,
  Loader2,
  MessageSquareText,
  Search,
  Trash2,
  XCircle,
} from "lucide-react";
import { toast } from "sonner";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { useDeleteSmsLog, useSmsDashboardStats, useSmsLogs } from "../hooks/useSmsData";
import type { SmsLogDto, SmsStatus } from "../types/sms.types";

const PAGE_SIZE = 15;

const STATUS_OPTIONS: { value: SmsStatus; label: string }[] = [
  { value: 1, label: "Pending" },
  { value: 2, label: "Sent" },
  { value: 3, label: "Delivered" },
  { value: 4, label: "Failed" },
];

function DateFilterField({
  label,
  value,
  min,
  max,
  onChange,
}: {
  label: string;
  value: string;
  min?: string;
  max?: string;
  onChange: (value: string) => void;
}) {
  const inputRef = useRef<HTMLInputElement>(null);

  function openCalendar() {
    const input = inputRef.current;
    if (!input) return;

    if (typeof input.showPicker === "function") {
      input.showPicker();
    } else {
      input.focus();
      input.click();
    }
  }

  return (
    <div>
      <label className="mb-1 block text-xs font-medium text-muted-foreground">
        {label}
      </label>
      <div className="relative">
        <input
          ref={inputRef}
          type="date"
          aria-label={label}
          value={value}
          min={min}
          max={max}
          onChange={(event) => onChange(event.target.value)}
          className="w-full rounded-md border bg-background px-3 py-2 pr-10"
        />
        <button
          type="button"
          onClick={openCalendar}
          aria-label={`Open ${label.toLowerCase()} calendar`}
          className="absolute right-2 top-1/2 -translate-y-1/2 rounded p-1 text-muted-foreground transition-colors hover:bg-accent hover:text-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
        >
          <CalendarDays aria-hidden="true" className="h-4 w-4" />
        </button>
      </div>
    </div>
  );
}

function formatDateTime(value: string | null) {
  if (!value) return "—";
  const date = new Date(value);
  return Number.isNaN(date.getTime())
    ? "—"
    : new Intl.DateTimeFormat(undefined, {
        dateStyle: "medium",
        timeStyle: "short",
      }).format(date);
}

function SmsStatusBadge({ status }: { status: SmsStatus }) {
  const config = {
    1: {
      label: "Pending",
      icon: Clock3,
      className: "border-amber-200 bg-amber-50 text-amber-800",
    },
    2: {
      label: "Sent",
      icon: CheckCircle2,
      className: "border-sky-200 bg-sky-50 text-sky-800",
    },
    3: {
      label: "Delivered",
      icon: CheckCircle2,
      className: "border-emerald-200 bg-emerald-50 text-emerald-800",
    },
    4: {
      label: "Failed",
      icon: XCircle,
      className: "border-rose-200 bg-rose-50 text-rose-800",
    },
  }[status];
  const Icon = config.icon;
  return (
    <Badge variant="outline" className={config.className}>
      <Icon aria-hidden="true" className="mr-1 h-3.5 w-3.5" />
      {config.label}
    </Badge>
  );
}

function StatCard({
  label,
  value,
  hint,
  tone = "default",
}: {
  label: string;
  value: string | number;
  hint?: string;
  tone?: "default" | "success" | "warning" | "danger";
}) {
  const tones = {
    default: "text-foreground",
    success: "text-emerald-700",
    warning: "text-amber-700",
    danger: "text-rose-700",
  };
  return (
    <Card>
      <CardContent className="p-4 sm:p-5">
        <p className="text-sm text-muted-foreground">{label}</p>
        <p className={`mt-2 text-2xl font-semibold tabular-nums ${tones[tone]}`}>
          {value}
        </p>
        {hint && <p className="mt-1 text-xs text-muted-foreground">{hint}</p>}
      </CardContent>
    </Card>
  );
}

export function SmsLogsPage({ embedded = false }: { embedded?: boolean }) {
  const { hasPermission } = usePermissions();
  const canView = hasPermission(Permission.SmsLogView);
  const canDelete = hasPermission(Permission.SmsLogDelete);
  const [search, setSearch] = useState("");
  const [recipient, setRecipient] = useState("");
  const [status, setStatus] = useState("");
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [page, setPage] = useState(1);
  const [expandedId, setExpandedId] = useState<number | null>(null);
  const filters = useMemo(
    () => ({
      pageNumber: page,
      pageSize: PAGE_SIZE,
      searchTerm: search.trim() || undefined,
      recipientNumber: recipient.trim() || undefined,
      status: status ? (Number(status) as SmsStatus) : undefined,
      fromDate: fromDate || undefined,
      toDate: toDate || undefined,
      sortBy: "CreatedAt",
      sortDescending: true,
    }),
    [fromDate, page, recipient, search, status, toDate]
  );
  const logsQuery = useSmsLogs(filters, canView);
  const statsQuery = useSmsDashboardStats(canView);
  const deleteLog = useDeleteSmsLog();

  async function removeLog(item: SmsLogDto) {
    if (!window.confirm(`Delete SMS log for ${item.recipientNumber}?`)) return;
    try {
      await deleteLog.mutateAsync(item.id);
      toast.success("SMS log deleted.");
      if (expandedId === item.id) setExpandedId(null);
    } catch (error) {
      toast.error(
        error instanceof Error ? error.message : "Unable to delete SMS log."
      );
    }
  }

  function resetFilters() {
    setSearch("");
    setRecipient("");
    setStatus("");
    setFromDate("");
    setToDate("");
    setPage(1);
  }

  if (!canView) {
    return (
      <Card className="mx-auto max-w-4xl">
        <CardContent className="p-6 text-sm text-destructive">
          You do not have permission to view SMS logs.
        </CardContent>
      </Card>
    );
  }

  const logs = logsQuery.data?.items ?? [];
  const stats = statsQuery.data;

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      {!embedded && (
      <header className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">SMS logs</h1>
          <p className="mt-1 text-sm text-muted-foreground">
            Review delivery history, status, recipient and provider response.
          </p>
        </div>
        <Link
          to="/sms/templates"
          className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent"
        >
          Manage templates
        </Link>
      </header>
      )}

      {statsQuery.isPending ? (
        <div className="flex items-center gap-2 text-sm text-muted-foreground">
          <Loader2 aria-hidden="true" className="h-4 w-4 animate-spin" />
          Loading SMS summary…
        </div>
      ) : statsQuery.isError ? (
        <div className="flex flex-col gap-2 rounded-lg border border-destructive/30 bg-destructive/5 p-4 text-sm sm:flex-row sm:items-center sm:justify-between">
          <p className="text-destructive">
            SMS summary could not be loaded.{" "}
            {statsQuery.error instanceof Error ? statsQuery.error.message : ""}
          </p>
          <Button variant="outline" onClick={() => void statsQuery.refetch()}>
            Retry summary
          </Button>
        </div>
      ) : stats ? (
        <section aria-label="SMS delivery summary" className="space-y-3">
          <div className="grid gap-3 grid-cols-2 md:grid-cols-4 xl:grid-cols-7">
            <StatCard label="All messages" value={stats.totalSms} />
            <StatCard label="Today" value={stats.todaySms} />
            <StatCard label="This week" value={stats.weeklySms} />
            <StatCard label="This month" value={stats.monthlySms} />
            <StatCard
              label="Success rate"
              value={`${Math.max(0, Math.min(100, stats.successRate)).toFixed(1)}%`}
              hint={`${stats.successCount} sent or delivered`}
              tone="success"
            />
            <StatCard label="Failed" value={stats.failedCount} tone="danger" />
            <StatCard label="Pending" value={stats.pendingCount} tone="warning" />
          </div>
          <div
            className="h-2 overflow-hidden rounded-full bg-muted"
            role="progressbar"
            aria-label="SMS success rate"
            aria-valuemin={0}
            aria-valuemax={100}
            aria-valuenow={Math.max(0, Math.min(100, stats.successRate))}
          >
            <div
              className="h-full rounded-full bg-emerald-500 transition-[width]"
              style={{
                width: `${Math.max(0, Math.min(100, stats.successRate))}%`,
              }}
            />
          </div>
        </section>
      ) : null}

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <MessageSquareText aria-hidden="true" className="h-5 w-5 text-primary" />
            Message history
          </CardTitle>
          <CardDescription>
            Filter by message text, exact recipient, delivery status or date range.
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-5">
            <label className="relative sm:col-span-2 lg:col-span-1">
              <span className="sr-only">Search SMS content</span>
              <Search aria-hidden="true" className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <input
                value={search}
                onChange={(event) => {
                  setSearch(event.target.value);
                  setPage(1);
                }}
                className="w-full rounded-md border bg-background py-2 pl-9 pr-3"
                placeholder="Search message"
              />
            </label>
            <label>
              <span className="sr-only">Recipient phone number</span>
              <input
                value={recipient}
                onChange={(event) => {
                  setRecipient(event.target.value);
                  setPage(1);
                }}
                className="w-full rounded-md border bg-background px-3 py-2"
                placeholder="Recipient number"
                inputMode="tel"
              />
            </label>
            <label>
              <span className="sr-only">Filter by SMS status</span>
              <select
                value={status}
                onChange={(event) => {
                  setStatus(event.target.value);
                  setPage(1);
                }}
                className="w-full rounded-md border bg-background px-3 py-2"
              >
                <option value="">All statuses</option>
                {STATUS_OPTIONS.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </label>
            <DateFilterField
              label="From date"
              value={fromDate}
              max={toDate || undefined}
              onChange={(value) => {
                setFromDate(value);
                setPage(1);
              }}
            />
            <DateFilterField
              label="To date"
              value={toDate}
              min={fromDate || undefined}
              onChange={(value) => {
                setToDate(value);
                setPage(1);
              }}
            />
          </div>
          <div className="flex justify-end">
            <Button variant="outline" onClick={resetFilters}>
              Clear filters
            </Button>
          </div>

          {logsQuery.isPending ? (
            <div className="flex items-center justify-center gap-2 py-12 text-sm text-muted-foreground">
              <Loader2 aria-hidden="true" className="h-5 w-5 animate-spin" />
              Loading SMS logs…
            </div>
          ) : logsQuery.isError ? (
            <div className="rounded-lg border border-destructive/30 bg-destructive/5 p-5 text-sm">
              <p className="font-medium text-destructive">SMS logs could not be loaded.</p>
              <p className="mt-1 text-muted-foreground">
                {logsQuery.error instanceof Error
                  ? logsQuery.error.message
                  : "Please try again."}
              </p>
              <Button className="mt-3" variant="outline" onClick={() => void logsQuery.refetch()}>
                Try again
              </Button>
            </div>
          ) : logs.length === 0 ? (
            <div className="rounded-lg border border-dashed px-5 py-12 text-center">
              <AlertCircle aria-hidden="true" className="mx-auto h-8 w-8 text-muted-foreground" />
              <p className="mt-3 font-medium">No SMS logs found</p>
              <p className="mt-1 text-sm text-muted-foreground">
                Clear or adjust the filters to see matching message history.
              </p>
            </div>
          ) : (
            <>
              <div className="space-y-3">
                {logs.map((item) => {
                  const expanded = expandedId === item.id;
                  return (
                    <article key={item.id} className="rounded-lg border p-4">
                      <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                        <div className="min-w-0 flex-1">
                          <div className="flex flex-wrap items-center gap-2">
                            <h3 className="font-semibold tabular-nums">{item.recipientNumber}</h3>
                            <SmsStatusBadge status={item.status} />
                          </div>
                          <p className={`mt-2 whitespace-pre-wrap break-words text-sm ${expanded ? "" : "line-clamp-2"}`}>
                            {item.message}
                          </p>
                          <div className="mt-3 flex flex-wrap gap-x-4 gap-y-1 text-xs text-muted-foreground">
                            <span>{item.studentName ? `Student: ${item.studentName}` : "No linked student"}</span>
                            <span>{item.provider || "Provider not recorded"}</span>
                            <span>{formatDateTime(item.sentAt ?? item.createdAt)}</span>
                          </div>
                        </div>
                        <div className="flex shrink-0 gap-2">
                          <Button
                            variant="outline"
                            aria-expanded={expanded}
                            onClick={() => setExpandedId(expanded ? null : item.id)}
                          >
                            {expanded ? "Hide details" : "Details"}
                          </Button>
                          {canDelete && (
                            <Button
                              variant="outline"
                              className="text-destructive hover:bg-destructive/10"
                              aria-label={`Delete log for ${item.recipientNumber}`}
                              disabled={deleteLog.isPending}
                              onClick={() => void removeLog(item)}
                            >
                              <Trash2 aria-hidden="true" className="h-4 w-4" />
                            </Button>
                          )}
                        </div>
                      </div>
                      {expanded && (
                        <div className="mt-4 grid gap-3 border-t pt-4 text-sm sm:grid-cols-2">
                          <div>
                            <p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
                              Created
                            </p>
                            <p className="mt-1">{formatDateTime(item.createdAt)}</p>
                          </div>
                          <div>
                            <p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
                              Sent
                            </p>
                            <p className="mt-1">{formatDateTime(item.sentAt)}</p>
                          </div>
                          {item.studentId !== null && (
                            <div>
                              <p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
                                Student ID
                              </p>
                              <p className="mt-1">{item.studentId}</p>
                            </div>
                          )}
                          <div className="sm:col-span-2">
                            <p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
                              Provider response
                            </p>
                            <p className="mt-1 whitespace-pre-wrap break-words">
                              {item.providerResponse || "No provider response recorded."}
                            </p>
                          </div>
                        </div>
                      )}
                    </article>
                  );
                })}
              </div>
              {logsQuery.data && logsQuery.data.totalPages > 1 && (
                <div className="flex items-center justify-between border-t pt-4">
                  <p className="text-sm text-muted-foreground">
                    Page {logsQuery.data.pageNumber} of {logsQuery.data.totalPages}
                    <span className="hidden sm:inline"> · {logsQuery.data.totalCount} messages</span>
                  </p>
                  <div className="flex gap-2">
                    <Button
                      variant="outline"
                      disabled={!logsQuery.data.hasPreviousPage}
                      onClick={() => setPage((value) => Math.max(1, value - 1))}
                    >
                      Previous
                    </Button>
                    <Button
                      variant="outline"
                      disabled={!logsQuery.data.hasNextPage}
                      onClick={() => setPage((value) => value + 1)}
                    >
                      Next
                    </Button>
                  </div>
                </div>
              )}
            </>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
