import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  assignPermissionsToRole,
  assignRoleToUser,
  getPermissions,
  getRoles,
  getRolePermissions,
  getUserPermissions,
  getUserRoles,
  getUsers,
  removePermissionFromRole,
  removeRoleFromUser,
} from "../api/role.api";
import type {
  AssignPermissionsToRoleDto,
  AssignRoleToUserDto,
} from "../types/role.types";

export function useAccessUsers(enabled: boolean) {
  return useQuery({
    queryKey: ["role-access", "users"],
    queryFn: getUsers,
    enabled,
    staleTime: 30_000,
  });
}

export function useAccessRoles(enabled: boolean) {
  return useQuery({
    queryKey: ["role-access", "roles"],
    queryFn: getRoles,
    enabled,
    staleTime: 30_000,
  });
}

export function useAccessPermissions(enabled: boolean) {
  return useQuery({
    queryKey: ["role-access", "permission-catalog"],
    queryFn: getPermissions,
    enabled,
    staleTime: 30_000,
  });
}

export function useRolePermissionAssignments(roleId: number | null, enabled: boolean) {
  return useQuery({
    queryKey: ["role-access", "roles", roleId, "permissions"],
    queryFn: () => getRolePermissions(roleId as number),
    enabled: enabled && roleId !== null,
    staleTime: 30_000,
  });
}

export function useUserRoles(userId: number | null, enabled: boolean) {
  return useQuery({
    queryKey: ["role-access", "users", userId, "roles"],
    queryFn: () => getUserRoles(userId as number),
    enabled: enabled && userId !== null,
    staleTime: 30_000,
  });
}

export function useUserPermissions(userId: number | null, enabled: boolean) {
  return useQuery({
    queryKey: ["role-access", "user-permissions", userId],
    queryFn: () => getUserPermissions(userId as number),
    enabled: enabled && userId !== null,
    staleTime: 30_000,
  });
}

function invalidateUserAccess(
  queryClient: ReturnType<typeof useQueryClient>,
  userId: number
) {
  void queryClient.invalidateQueries({
    queryKey: ["role-access", "users", userId, "roles"],
  });
  void queryClient.invalidateQueries({
    queryKey: ["role-access", "user-permissions", userId],
  });
  void queryClient.invalidateQueries({
    queryKey: ["current-user", "profile", userId],
  });
}

export function useAssignRoleToUser() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: AssignRoleToUserDto) => assignRoleToUser(payload),
    onSuccess: (_data, payload) => invalidateUserAccess(queryClient, payload.userId),
  });
}

export function useRemoveRoleFromUser() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ userId, roleId }: { userId: number; roleId: number }) =>
      removeRoleFromUser(userId, roleId),
    onSuccess: (_data, payload) => invalidateUserAccess(queryClient, payload.userId),
  });
}

export function useAssignPermissionsToRole() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: AssignPermissionsToRoleDto) =>
      assignPermissionsToRole(payload),
    onSettled: (_data, _error, payload) => {
      void queryClient.invalidateQueries({
        queryKey: ["role-access", "roles", payload.roleId, "permissions"],
      });
      void queryClient.invalidateQueries({
        queryKey: ["role-access", "user-permissions"],
      });
      void queryClient.invalidateQueries({
        queryKey: ["current-user", "profile"],
      });
    },
  });
}

export function useRemovePermissionFromRole() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ roleId, permissionId }: { roleId: number; permissionId: number }) =>
      removePermissionFromRole(roleId, permissionId),
    onSettled: (_data, _error, payload) => {
      void queryClient.invalidateQueries({
        queryKey: ["role-access", "roles", payload.roleId, "permissions"],
      });
      void queryClient.invalidateQueries({
        queryKey: ["role-access", "user-permissions"],
      });
      void queryClient.invalidateQueries({
        queryKey: ["current-user", "profile"],
      });
    },
  });
}
