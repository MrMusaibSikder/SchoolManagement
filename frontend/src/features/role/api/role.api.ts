import { authApiClient } from "@/lib/api/auth-client";
import type {
  AssignPermissionsToRoleDto,
  CreatePermissionDto,
  CreateRoleDto,
  PermissionDto,
  RoleDto,
  UpdatePermissionDto,
  UpdateRoleDto,
} from "../types/role.types";

export async function getRoles(): Promise<RoleDto[]> {
  const { data } = await authApiClient.get<RoleDto[]>("/Role");
  return data;
}

export async function getRoleById(id: number): Promise<RoleDto> {
  const { data } = await authApiClient.get<RoleDto>(`/Role/${id}`);
  return data;
}

export async function createRole(payload: CreateRoleDto): Promise<RoleDto> {
  const { data } = await authApiClient.post<RoleDto>("/Role", payload);
  return data;
}

export async function updateRole(id: number, payload: UpdateRoleDto): Promise<RoleDto> {
  const { data } = await authApiClient.put<RoleDto>(`/Role/${id}`, payload);
  return data;
}

export async function deleteRole(id: number): Promise<void> {
  await authApiClient.delete(`/Role/${id}`);
}

export async function getPermissions(): Promise<PermissionDto[]> {
  const { data } = await authApiClient.get<PermissionDto[]>("/Permission");
  return data;
}

export async function createPermission(payload: CreatePermissionDto): Promise<PermissionDto> {
  const { data } = await authApiClient.post<PermissionDto>("/Permission", payload);
  return data;
}

export async function updatePermission(id: number, payload: UpdatePermissionDto): Promise<PermissionDto> {
  const { data } = await authApiClient.put<PermissionDto>(`/Permission/${id}`, payload);
  return data;
}

export async function deletePermission(id: number): Promise<void> {
  await authApiClient.delete(`/Permission/${id}`);
}

export async function getRolePermissions(roleId: number): Promise<PermissionDto[]> {
  const { data } = await authApiClient.get<PermissionDto[]>(`/Role/${roleId}/permissions`);
  return data;
}

export async function assignPermissionsToRole(payload: AssignPermissionsToRoleDto): Promise<void> {
  await authApiClient.post("/Role/assign-permissions", {
    RoleId: payload.roleId,
    PermissionIds: payload.permissionIds,
  });
}
