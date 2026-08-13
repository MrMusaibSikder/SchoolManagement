using SchoolERP.Application.Features.Role.DTOs;

namespace SchoolERP.Application.Features.Role.Interfaces;

/// <summary>
/// Business/service contract for Role records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface IRoleService
{
    /// <summary>Retrieves every Role record.</summary>
    Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single Role record by id, or null if it does not exist.</summary>
    Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new Role record.</summary>
    Task<RoleDto> CreateAsync(CreateRoleDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing Role record.</summary>
    Task<RoleDto> UpdateAsync(int id, UpdateRoleDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing Role record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
