using CourseHub.Domain.Entities;

namespace CourseHub.Application.Common.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Full role list — used by the admin "Roles & Permissions" screen to
    /// let the caller pick a role before viewing/editing its permissions.
    /// </summary>
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Role role, CancellationToken cancellationToken = default);
}
