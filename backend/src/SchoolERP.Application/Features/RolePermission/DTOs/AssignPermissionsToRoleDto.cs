namespace SchoolERP.Application.Features.RolePermission.DTOs;

/// <summary>
/// Input model for the "Assign Permissions to Role" admin action. Associates a
/// batch of permissions with a single role in one call; permissions already
/// assigned to the role are silently skipped.
/// </summary>
public class AssignPermissionsToRoleDto
{
    public int RoleId { get; set; }

    public List<int> PermissionIds { get; set; } = new();
}
