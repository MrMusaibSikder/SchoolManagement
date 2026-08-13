using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.UserRole.Interfaces;

/// <summary>
/// Repository contract for the <see cref="UserRole"/> join entity, which uses a
/// composite key (UserId, RoleId) rather than the single-Id base entity shape,
/// so it is not built on the generic repository.
/// </summary>
public interface IUserRoleRepository
{
    /// <summary>Gets a single association by its composite key.</summary>
    Task<SchoolERP.Domain.Entities.UserRole?> GetAsync(int userId, int roleId, CancellationToken cancellationToken = default);

    /// <summary>Gets every association.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.UserRole>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Checks whether an association already exists.</summary>
    Task<bool> ExistsAsync(int userId, int roleId, CancellationToken cancellationToken = default);

    /// <summary>Adds a new association.</summary>
    Task<SchoolERP.Domain.Entities.UserRole> AddAsync(SchoolERP.Domain.Entities.UserRole entity, CancellationToken cancellationToken = default);

    /// <summary>Removes an existing association.</summary>
    void Remove(SchoolERP.Domain.Entities.UserRole entity);
}
