import { Link } from "react-router-dom";
import type { LucideIcon } from "lucide-react";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";

interface StatCardProps {
  title: string;
  value: string;
  description: string;
  icon: LucideIcon;
  color?: "green" | "gold" | "blue" | "violet" | "teal" | "rose";
}

export function DashboardStatCard({
  title,
  value,
  description,
  icon: Icon,
  color,
}: StatCardProps) {
  const colorStyles = color
    ? {
        green: {
          border: "border-t-emerald-500",
          icon: "bg-emerald-100 text-emerald-800 dark:bg-emerald-900/70 dark:text-emerald-200",
        },
        gold: {
          border: "border-t-amber-400",
          icon: "bg-amber-100 text-amber-800 dark:bg-amber-900/70 dark:text-amber-200",
        },
        blue: {
          border: "border-t-sky-500",
          icon: "bg-sky-100 text-sky-800 dark:bg-sky-900/70 dark:text-sky-200",
        },
        violet: {
          border: "border-t-violet-400",
          icon: "bg-violet-100 text-violet-800 dark:bg-violet-900/70 dark:text-violet-200",
        },
        teal: {
          border: "border-t-teal-500",
          icon: "bg-teal-100 text-teal-800 dark:bg-teal-900/70 dark:text-teal-200",
        },
        rose: {
          border: "border-t-rose-400",
          icon: "bg-rose-100 text-rose-800 dark:bg-rose-900/70 dark:text-rose-200",
        },
      }[color]
    : null;

  return (
    <Card className={colorStyles ? `border-t-4 ${colorStyles.border}` : undefined}>
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <CardTitle className="text-sm font-medium text-muted-foreground">
          {title}
        </CardTitle>
        <span className={`flex h-9 w-9 items-center justify-center rounded-lg ${colorStyles?.icon ?? "bg-muted text-muted-foreground"}`}>
          <Icon aria-hidden="true" className="h-5 w-5" />
        </span>
      </CardHeader>
      <CardContent>
        <div className="text-3xl font-semibold tabular-nums">{value}</div>
        <p className="mt-1 text-xs text-muted-foreground">{description}</p>
      </CardContent>
    </Card>
  );
}

interface QuickActionCardProps {
  label: string;
  description: string;
  to: string;
  icon: LucideIcon;
  color: "green" | "gold" | "blue" | "violet" | "teal" | "rose";
}

export function DashboardQuickActionCard({
  label,
  description,
  to,
  icon: Icon,
  color,
}: QuickActionCardProps) {
  const colors = {
    green: "border-t-emerald-500",
    gold: "border-t-amber-400",
    blue: "border-t-sky-500",
    violet: "border-t-violet-400",
    teal: "border-t-cyan-500",
    rose: "border-t-rose-400",
  }[color];

  return (
    <Link
      to={to}
      className="group rounded-lg focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
    >
      <Card
        className={`h-full min-h-28 border-t-4 transition duration-200 group-hover:-translate-y-0.5 group-hover:shadow-md ${colors}`}
      >
        <CardContent className="flex h-full items-start gap-4 p-5">
          <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary transition group-hover:bg-primary group-hover:text-primary-foreground">
            <Icon aria-hidden="true" className="h-5 w-5" />
          </span>
          <span className="min-w-0 flex-1">
            <span className="block font-semibold text-foreground">{label}</span>
            <span className="mt-1 block text-sm text-muted-foreground">
              {description}
            </span>
          </span>
        </CardContent>
      </Card>
    </Link>
  );
}
