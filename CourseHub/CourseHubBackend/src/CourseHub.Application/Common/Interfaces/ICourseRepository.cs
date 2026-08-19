using CourseHub.Domain.Entities;

namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Minimal, read-only slice needed by Phase 11's public endpoints.
/// Phase 12 will extend this same interface with the admin Courses CRUD.
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
}
