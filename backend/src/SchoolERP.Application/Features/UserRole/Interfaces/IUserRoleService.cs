using SchoolERP.Application.Features.UserRole.DTOs;

namespace SchoolERP.Application.Features.UserRole.Interfaces;

/// <summary>
/// Business/service contract for the UserRole association. Returns DTOs only.
/// </summary>
public interface IUserRoleService
{
    /// <summary>Retrieves every UserRole association.</summary>
    Task<IReadOnlyList<UserRoleDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single association by its composite key, or null if it does not exist.</summary>
    Task<UserRoleDto?> GetAsync(int userId, int roleId, CancellationToken cancellationToken = default);

    /// <summary>Creates a new UserRole association.</summary>
    Task<UserRoleDto> AssignAsync(UserRoleDto request, CancellationToken cancellationToken = default);

    /// <summary>Removes an existing UserRole association.</summary>
    Task RemoveAsync(int userId, int roleId, CancellationToken cancellationToken = default);
}
