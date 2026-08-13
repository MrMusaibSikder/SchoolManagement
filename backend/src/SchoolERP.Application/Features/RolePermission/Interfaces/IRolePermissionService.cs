using SchoolERP.Application.Features.RolePermission.DTOs;

namespace SchoolERP.Application.Features.RolePermission.Interfaces;

/// <summary>
/// Business/service contract for the RolePermission association. Returns DTOs only.
/// </summary>
public interface IRolePermissionService
{
    /// <summary>Retrieves every RolePermission association.</summary>
    Task<IReadOnlyList<RolePermissionDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single association by its composite key, or null if it does not exist.</summary>
    Task<RolePermissionDto?> GetAsync(int roleId, int permissionId, CancellationToken cancellationToken = default);

    /// <summary>Creates a new RolePermission association.</summary>
    Task<RolePermissionDto> AssignAsync(RolePermissionDto request, CancellationToken cancellationToken = default);

    /// <summary>Removes an existing RolePermission association.</summary>
    Task RemoveAsync(int roleId, int permissionId, CancellationToken cancellationToken = default);
}
