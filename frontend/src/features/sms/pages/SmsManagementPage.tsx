import { useEffect, useMemo, useState } from "react";
import { Link, useLocation } from "react-router-dom";
import { CalendarDays, Clock3, FileText, MessageSquareText } from "lucide-react";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { cn } from "@/lib/utils";
import { Permission } from "@/lib/permissions";
import { SmsLogsPage } from "./SmsLogsPage";
import { SmsTemplatesPage } from "./SmsTemplatesPage";

type SmsTab = "templates" | "logs";

export function SmsManagementPage() {
  const location = useLocation();
  const [currentTime, setCurrentTime] = useState(() => new Date());
  const { hasPermission } = usePermissions();
  const canViewTemplates = hasPermission(Permission.SmsTemplateView);
  const canManageTemplates = hasPermission(Permission.SmsTemplateCreate)
    || hasPermission(Permission.SmsTemplateEdit)
    || hasPermission(Permission.SmsTemplateDelete);
  const canViewLogs = hasPermission(Permission.SmsLogView);
  const templateTabAvailable = canViewTemplates || canManageTemplates;
  const isLogsPath = location.pathname.endsWith("/logs");
  const activeTab: SmsTab =
    isLogsPath || (!templateTabAvailable && canViewLogs)
      ? "logs"
      : "templates";

  const tabs = useMemo(
    () =>
      [
        {
          id: "templates" as const,
          label: "Templates",
          icon: FileText,
          available: templateTabAvailable,
        },
        {
          id: "logs" as const,
          label: "SMS logs",
          icon: MessageSquareText,
          available: canViewLogs,
        },
      ].filter((tab) => tab.available),
    [canViewLogs, templateTabAvailable]
  );

  useEffect(() => {
    const intervalId = window.setInterval(() => {
      setCurrentTime(new Date());
    }, 1_000);

    return () => window.clearInterval(intervalId);
  }, []);

  const currentDate = new Intl.DateTimeFormat("en-BD", {
    timeZone: "Asia/Dhaka",
    weekday: "long",
    year: "numeric",
    month: "long",
    day: "numeric",
  }).format(currentTime);
  const currentClock = new Intl.DateTimeFormat("en-BD", {
    timeZone: "Asia/Dhaka",
    hour: "numeric",
    minute: "2-digit",
    second: "2-digit",
    hour12: true,
  }).format(currentTime);

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <header className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">SMS management</h1>
          <p className="mt-1 text-sm text-muted-foreground">
            Manage reusable message templates and review delivery history.
          </p>
        </div>
        <div
          aria-label={`Current date and time in Dhaka: ${currentDate}, ${currentClock}`}
          aria-live="off"
          className="flex items-center gap-3 rounded-lg border bg-card px-4 py-3 shadow-sm"
        >
          <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary">
            <CalendarDays aria-hidden="true" className="h-5 w-5" />
          </div>
          <div className="min-w-0">
            <p className="truncate text-sm font-medium">{currentDate}</p>
            <p className="mt-0.5 flex items-center gap-1.5 text-xs text-muted-foreground">
              <Clock3 aria-hidden="true" className="h-3.5 w-3.5" />
              <time dateTime={currentTime.toISOString()}>{currentClock}</time>
              <span>· Dhaka</span>
            </p>
          </div>
        </div>
      </header>

      <nav
        aria-label="SMS management sections"
        className="flex w-full gap-1 overflow-x-auto rounded-lg border bg-muted/40 p-1 sm:w-fit"
      >
        {tabs.map((tab) => {
          const Icon = tab.icon;
          const selected = activeTab === tab.id;
          return (
            <Link
              key={tab.id}
              to={tab.id === "logs" ? "/sms/logs" : "/sms/templates"}
              aria-current={selected ? "page" : undefined}
              className={cn(
                "inline-flex min-w-fit items-center justify-center gap-2 rounded-md px-4 py-2 text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring",
                selected
                  ? "bg-background text-foreground shadow-sm"
                  : "text-muted-foreground hover:bg-background/70 hover:text-foreground"
              )}
            >
              <Icon aria-hidden="true" className="h-4 w-4" />
              {tab.label}
            </Link>
          );
        })}
      </nav>

      <section
        aria-label={activeTab === "logs" ? "SMS logs" : "SMS templates"}
        className="space-y-6"
      >
        {activeTab === "logs" ? (
          <SmsLogsPage embedded />
        ) : (
          <SmsTemplatesPage embedded />
        )}
      </section>
    </div>
  );
}
