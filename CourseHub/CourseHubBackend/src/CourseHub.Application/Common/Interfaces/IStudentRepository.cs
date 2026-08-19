namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Minimal, read-only slice needed by Phase 11's public stats endpoint.
/// Phase 12 will extend this with the admin Students CRUD. Students are
/// never listed publicly (unlike Teachers/Courses) — only an aggregate
/// count is ever exposed, for privacy.
/// </summary>
public interface IStudentRepository
{
    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);
}
