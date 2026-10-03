import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  assignPermissionsToRole,
  createPermission,
  createRole,
  deletePermission,
  deleteRole,
  getPermissions,
  getRolePermissions,
  getRoles,
  updatePermission,
  updateRole,
} from "../api/role.api";
import type {
  AssignPermissionsToRoleDto,
  CreatePermissionDto,
  CreateRoleDto,
  UpdatePermissionDto,
  UpdateRoleDto,
} from "../types/role.types";

export function useRoles() {
  return useQuery({
    queryKey: ["roles"],
    queryFn: getRoles,
    staleTime: 30_000,
  });
}

export function usePermissionsCatalog() {
  return useQuery({
    queryKey: ["permissions"],
    queryFn: getPermissions,
    staleTime: 30_000,
  });
}

export function useRolePermissions(roleId: number | null) {
  return useQuery({
    queryKey: ["roles", roleId, "permissions"],
    queryFn: () => getRolePermissions(roleId as number),
    enabled: Boolean(roleId),
    staleTime: 30_000,
  });
}

export function useCreateRole() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateRoleDto) => createRole(payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["roles"] });
    },
  });
}

export function useUpdateRole() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateRoleDto }) => updateRole(id, payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["roles"] });
    },
  });
}

export function useDeleteRole() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => deleteRole(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["roles"] });
      void queryClient.invalidateQueries({ queryKey: ["roles", "permissions"] });
    },
  });
}

export function useCreatePermission() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreatePermissionDto) => createPermission(payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["permissions"] });
    },
  });
}

export function useUpdatePermission() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdatePermissionDto }) => updatePermission(id, payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["permissions"] });
    },
  });
}

export function useDeletePermission() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => deletePermission(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["permissions"] });
    },
  });
}

export function useAssignPermissionsToRole() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: AssignPermissionsToRoleDto) => assignPermissionsToRole(payload),
    onSuccess: (_data, variables) => {
      void queryClient.invalidateQueries({ queryKey: ["roles", variables.roleId, "permissions"] });
    },
  });
}
