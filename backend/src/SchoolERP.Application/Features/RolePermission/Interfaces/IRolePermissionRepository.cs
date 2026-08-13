using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.RolePermission.Interfaces;

/// <summary>
/// Repository contract for the <see cref="RolePermission"/> join entity, which uses a
/// composite key (RoleId, PermissionId) rather than the single-Id base entity shape,
/// so it is not built on the generic repository.
/// </summary>
public interface IRolePermissionRepository
{
    /// <summary>Gets a single association by its composite key.</summary>
    Task<SchoolERP.Domain.Entities.RolePermission?> GetAsync(int roleId, int permissionId, CancellationToken cancellationToken = default);

    /// <summary>Gets every association.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.RolePermission>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Checks whether an association already exists.</summary>
    Task<bool> ExistsAsync(int roleId, int permissionId, CancellationToken cancellationToken = default);

    /// <summary>Adds a new association.</summary>
    Task<SchoolERP.Domain.Entities.RolePermission> AddAsync(SchoolERP.Domain.Entities.RolePermission entity, CancellationToken cancellationToken = default);

    /// <summary>Removes an existing association.</summary>
    void Remove(SchoolERP.Domain.Entities.RolePermission entity);
}
