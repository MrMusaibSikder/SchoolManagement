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

    /// <summary>
    /// Every user currently holding the given role — used to build the
    /// "eligible users" dropdown for promoting a user into a Teacher/
    /// Student profile (see ITeacherService/IStudentService.GetEligibleUsersAsync).
    /// </summary>
    Task<IReadOnlyList<User>> GetByRoleAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Admin user directory: optional name/email search, paged. Every
    /// user regardless of Status (Active/Inactive/Suspended) — this is
    /// the admin management screen, so inactive/suspended accounts must
    /// still be visible to manage.
    /// </summary>
    Task<(IReadOnlyList<User> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);
}
