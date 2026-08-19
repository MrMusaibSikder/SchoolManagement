using CourseHub.Domain.Entities;

namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Data access abstraction for User. CourseHub is single-institute, so
/// email is globally unique — no tenant scoping needed on lookups.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);
}
