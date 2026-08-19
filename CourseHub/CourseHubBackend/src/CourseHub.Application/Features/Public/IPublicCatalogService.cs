namespace CourseHub.Application.Features.Public;

/// <summary>
/// Phase 11: unauthenticated, public-facing catalog data — the public
/// "our teachers" / "our courses" pages and the landing-page stats/graph
/// numbers. Kept separate from IPublicInstitutionService (which only
/// covers the single Institution branding profile) since this reads
/// across several different aggregates (Teacher, Course, Student, Batch,
/// Enrollment) instead of one.
/// </summary>
public interface IPublicCatalogService
{
    Task<IReadOnlyList<PublicTeacherResponse>> GetPublicTeachersAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PublicCourseResponse>> GetPublicCoursesAsync(CancellationToken cancellationToken = default);

    Task<InstitutionStatsResponse> GetStatsAsync(CancellationToken cancellationToken = default);
}
