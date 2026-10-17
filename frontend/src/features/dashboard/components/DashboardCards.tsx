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
  const colorAccent = color
    ? {
        green: "border-t-emerald-500",
        gold: "border-t-amber-400",
        blue: "border-t-sky-500",
        violet: "border-t-violet-400",
        teal: "border-t-cyan-500",
        rose: "border-t-rose-400",
      }[color]
    : "";

  return (
    <Card className={color ? `border-t-4 ${colorAccent}` : undefined}>
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <CardTitle className="text-sm font-medium text-muted-foreground">
          {title}
        </CardTitle>
        <span className={`flex h-9 w-9 items-center justify-center rounded-lg ${color ? "bg-primary/10 text-primary" : "bg-muted text-muted-foreground"}`}>
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
