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
}

export function DashboardStatCard({
  title,
  value,
  description,
  icon: Icon,
}: StatCardProps) {
  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <CardTitle className="text-sm font-medium text-muted-foreground">
          {title}
        </CardTitle>
        <span className="flex h-9 w-9 items-center justify-center rounded-lg bg-muted text-muted-foreground">
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
}

export function DashboardQuickActionCard({
  label,
  description,
  to,
}: QuickActionCardProps) {
  return (
    <Link
      to={to}
      className="block rounded-lg border p-4 transition hover:border-primary/40 hover:bg-muted/40"
    >
      <p className="font-medium">{label}</p>
      <p className="mt-1 text-sm text-muted-foreground">{description}</p>
    </Link>
  );
}
