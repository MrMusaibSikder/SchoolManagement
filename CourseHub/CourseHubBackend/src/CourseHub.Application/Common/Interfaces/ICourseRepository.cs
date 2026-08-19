using CourseHub.Domain.Entities;

namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Phase 11 added the read-only public-catalog slice (GetPublicListAsync,
/// CountActiveAsync). Phase 12 extends it here with the full set needed
/// by the admin Courses CRUD.
/// </summary>
public interface ICourseRepository
{
    /// <summary>
    /// Active courses the institute has marked public (Course.IsPublic) —
    /// what shows on the public course catalog page.
    /// </summary>
    Task<IReadOnlyList<Course>> GetPublicListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Count of all active courses (public or not) — aggregate stat only.
    /// </summary>
    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);

    Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Course.Code has a unique DB index (see CourseConfiguration).
    /// <paramref name="excludingId"/> lets an Update check "does any
    /// *other* course already use this code" without the course's own
    /// unchanged code tripping a false positive.
    /// </summary>
    Task<bool> ExistsByCodeAsync(string code, Guid? excludingId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Admin course listing: optional name/code search, paged. Returns
    /// every course regardless of IsActive/IsPublic — unlike
    /// GetPublicListAsync, this is for the admin screen where inactive/
    /// unpublished courses must still be visible to manage.
    /// </summary>
    Task<(IReadOnlyList<Course> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(Course course, CancellationToken cancellationToken = default);
}
