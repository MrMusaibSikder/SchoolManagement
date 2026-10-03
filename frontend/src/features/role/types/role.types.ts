export interface RoleDto {
  id: number;
  name: string;
  description?: string | null;
}

export interface PermissionDto {
  id: number;
  name: string;
}

export interface CreateRoleDto {
  name: string;
  description?: string | null;
}

export interface UpdateRoleDto {
  id: number;
  name: string;
  description?: string | null;
}

export interface AssignPermissionsToRoleDto {
  roleId: number;
  permissionIds: number[];
}

export interface CreatePermissionDto {
  name: string;
}

export interface UpdatePermissionDto {
  id: number;
  name: string;
}
