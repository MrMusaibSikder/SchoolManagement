using CourseHub.Domain.Entities;

namespace CourseHub.Application.Common.Interfaces;

public interface IUserRoleRepository
{
    Task AddAsync(UserRole userRole, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Names of every role currently assigned to a user (via UserRole ->
    /// Role), for building safe response DTOs. Not an authorization check —
    /// permission evaluation (Phase 9) is a separate concern.
    /// </summary>
    Task<IReadOnlyList<string>> GetRoleNamesForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
