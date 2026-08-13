using SchoolERP.Application.Features.Permission.DTOs;
using SchoolERP.Application.Features.Role.DTOs;

namespace SchoolERP.Application.Features.Authorization.Interfaces;

/// <summary>
/// Read-only query service that resolves the effective roles and permissions for
/// a given user (roles assigned directly to the user, and permissions granted to
/// those roles). Used by the "Get User Roles" / "Get User Permissions" admin
/// endpoints, the Current User Profile endpoints, and the permission-based
/// authorization handler.
/// </summary>
public interface IUserAccessService
{
    /// <summary>Gets every role assigned to the given user.</summary>
    Task<IReadOnlyList<RoleDto>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the distinct, effective set of permissions granted to the given user
    /// through all of their assigned roles.
    /// </summary>
    Task<IReadOnlyList<PermissionDto>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether the given user has the specified permission, through any of
    /// their assigned roles. This is the primary check used by permission-based
    /// authorization.
    /// </summary>
    Task<bool> HasPermissionAsync(int userId, string permissionName, CancellationToken cancellationToken = default);
}
