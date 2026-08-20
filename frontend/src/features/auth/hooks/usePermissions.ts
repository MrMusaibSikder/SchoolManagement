import { useCallback, useMemo } from "react";
import { AppRole, type AppRoleName } from "@/lib/permissions";
import { useCurrentUser } from "./useCurrentUser";

function normalize(value: string) {
  return value.trim().toLowerCase();
}

export function usePermissions() {
  const { data: profile, isPending, isError } = useCurrentUser();

  const permissions = profile?.permissions ?? [];
  const roles = profile?.roles ?? [];

  const permissionSet = useMemo(
    () => new Set(permissions.map(normalize)),
    [permissions]
  );

  const roleSet = useMemo(() => new Set(roles.map(normalize)), [roles]);

  const hasPermission = useCallback(
    (permission: string) => permissionSet.has(normalize(permission)),
    [permissionSet]
  );

  const hasAnyPermission = useCallback(
    (required: string[]) => required.some((permission) => hasPermission(permission)),
    [hasPermission]
  );

  const hasAllPermissions = useCallback(
    (required: string[]) => required.every((permission) => hasPermission(permission)),
    [hasPermission]
  );

  const hasRole = useCallback(
    (role: AppRoleName | string) => roleSet.has(normalize(role)),
    [roleSet]
  );

  const primaryRole = useMemo(() => {
    const priority: AppRoleName[] = [
      AppRole.Admin,
      AppRole.Accountant,
      AppRole.Teacher,
      AppRole.Student,
    ];

    for (const role of priority) {
      if (hasRole(role)) return role;
    }

    return roles[0] ?? "User";
  }, [hasRole, roles]);

  const isAdmin = hasRole(AppRole.Admin);

  return {
    profile,
    permissions,
    roles,
    primaryRole,
    isAdmin,
    isPending,
    isError,
    hasPermission,
    hasAnyPermission,
    hasAllPermissions,
    hasRole,
  };
}
