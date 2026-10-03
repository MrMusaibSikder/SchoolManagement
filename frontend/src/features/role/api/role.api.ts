import { authApiClient } from "@/lib/api/auth-client";
import type {
  AssignPermissionsToRoleDto,
  AssignRoleToUserDto,
  PermissionDto,
  RoleDto,
  UserAccessDto,
} from "../types/role.types";

export async function getUsers(): Promise<UserAccessDto[]> {
  const { data } = await authApiClient.get<UserAccessDto[]>("/User");
  return data;
}

export async function getRoles(): Promise<RoleDto[]> {
  const { data } = await authApiClient.get<RoleDto[]>("/Role");
  return data;
}

export async function getPermissions(): Promise<PermissionDto[]> {
  const { data } = await authApiClient.get<PermissionDto[]>("/Permission");
  return data;
}

export async function getRolePermissions(roleId: number): Promise<PermissionDto[]> {
  const { data } = await authApiClient.get<PermissionDto[]>(`/Role/${roleId}/permissions`);
  return data;
}

export async function getUserRoles(userId: number): Promise<RoleDto[]> {
  const { data } = await authApiClient.get<RoleDto[]>(`/User/${userId}/roles`);
  return data;
}

export async function getUserPermissions(userId: number): Promise<PermissionDto[]> {
  const { data } = await authApiClient.get<PermissionDto[]>(`/User/${userId}/permissions`);
  return data;
}

export async function assignRoleToUser(payload: AssignRoleToUserDto): Promise<void> {
  await authApiClient.post("/User/assign-role", {
    UserId: payload.userId,
    RoleId: payload.roleId,
  });
}

export async function removeRoleFromUser(userId: number, roleId: number): Promise<void> {
  await authApiClient.delete(`/User/${userId}/roles/${roleId}`);
}

export async function assignPermissionsToRole(
  payload: AssignPermissionsToRoleDto
): Promise<void> {
  await authApiClient.post("/Role/assign-permissions", {
    RoleId: payload.roleId,
    PermissionIds: payload.permissionIds,
  });
}

export async function removePermissionFromRole(
  roleId: number,
  permissionId: number
): Promise<void> {
  await authApiClient.delete(`/Role/${roleId}/permissions/${permissionId}`);
}
