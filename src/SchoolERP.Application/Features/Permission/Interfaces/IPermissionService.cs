using SchoolERP.Application.Features.Permission.DTOs;

namespace SchoolERP.Application.Features.Permission.Interfaces;

/// <summary>
/// Business/service contract for Permission records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface IPermissionService
{
    /// <summary>Retrieves every Permission record.</summary>
    Task<IReadOnlyList<PermissionDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single Permission record by id, or null if it does not exist.</summary>
    Task<PermissionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new Permission record.</summary>
    Task<PermissionDto> CreateAsync(CreatePermissionDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing Permission record.</summary>
    Task<PermissionDto> UpdateAsync(int id, UpdatePermissionDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing Permission record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
