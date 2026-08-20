import type { ReactNode } from "react";
import type { AppRoleName } from "@/lib/permissions";
import { usePermissions } from "../hooks/usePermissions";

interface PermissionGuardProps {
  children: ReactNode;
  permission?: string;
  anyOf?: string[];
  allOf?: string[];
  role?: AppRoleName | string;
  fallback?: ReactNode;
  loadingFallback?: ReactNode;
}

/**
 * Renders children only when the current user satisfies the permission/role rule.
 * Authorization is enforced by the backend; this only hides unauthorized UI.
 */
export function PermissionGuard({
  children,
  permission,
  anyOf,
  allOf,
  role,
  fallback = null,
  loadingFallback = null,
}: PermissionGuardProps) {
  const {
    isPending,
    hasPermission,
    hasAnyPermission,
    hasAllPermissions,
    hasRole,
  } = usePermissions();

  if (isPending) return loadingFallback;

  if (permission && !hasPermission(permission)) return fallback;
  if (anyOf && !hasAnyPermission(anyOf)) return fallback;
  if (allOf && !hasAllPermissions(allOf)) return fallback;
  if (role && !hasRole(role)) return fallback;

  return children;
}
