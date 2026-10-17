import { useMemo } from "react";
import { Link, useLocation } from "react-router-dom";
import { FileText, MessageSquareText } from "lucide-react";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { cn } from "@/lib/utils";
import { Permission } from "@/lib/permissions";
import { SmsLogsPage } from "./SmsLogsPage";
import { SmsTemplatesPage } from "./SmsTemplatesPage";

type SmsTab = "templates" | "logs";

export function SmsManagementPage() {
  const location = useLocation();
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

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <header>
        <h1 className="font-display text-2xl font-semibold">SMS management</h1>
        <p className="mt-1 text-sm text-muted-foreground">
          Manage reusable message templates and review delivery history.
        </p>
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
