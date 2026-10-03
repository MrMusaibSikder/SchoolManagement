export interface RoleDto {
  id: number;
  name: string;
  description?: string | null;
}

export interface PermissionDto {
  id: number;
  name: string;
}

export interface UserAccessDto {
  id: number;
  username: string;
  email: string;
  isActive: boolean;
}

export interface AssignRoleToUserDto {
  userId: number;
  roleId: number;
}

export interface AssignPermissionsToRoleDto {
  roleId: number;
  permissionIds: number[];
}
