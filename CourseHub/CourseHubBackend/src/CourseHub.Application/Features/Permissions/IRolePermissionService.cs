using CourseHub.Application.Features.Permissions.Dtos;

namespace CourseHub.Application.Features.Permissions;

/// <summary>
/// Orchestrates the Phase 9 admin use cases: browsing the global
/// permission catalog and assigning/removing permissions on a role.
/// </summary>
public interface IRolePermissionService
{
    Task<IReadOnlyList<PermissionResponse>> GetCatalogAsync(CancellationToken cancellationToken = default);

    Task<RolePermissionsResponse> GetPermissionsForRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<RolePermissionsResponse> AssignPermissionAsync(Guid roleId, AssignPermissionRequest request, CancellationToken cancellationToken = default);

    Task RemovePermissionAsync(Guid roleId, string permissionName, CancellationToken cancellationToken = default);
}
