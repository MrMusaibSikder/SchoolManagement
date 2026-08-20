import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { getDashboardData } from "../api/dashboard.api";
import { buildDashboardAccess } from "../config/dashboard-access";

export function useDashboardData() {
  const { permissions, isPending: permissionsPending } = usePermissions();

  const access = useMemo(
    () => buildDashboardAccess(permissions),
    [permissions]
  );

  return useQuery({
    queryKey: ["dashboard", "overview", permissions],
    queryFn: () => getDashboardData(access),
    enabled: !permissionsPending,
    staleTime: 60 * 1000,
    retry: 1,
  });
}

export function useDashboardAccess() {
  const { permissions, isPending, primaryRole, roles, hasPermission } =
    usePermissions();

  const access = useMemo(
    () => buildDashboardAccess(permissions),
    [permissions]
  );

  return { access, permissions, isPending, primaryRole, roles, hasPermission };
}
