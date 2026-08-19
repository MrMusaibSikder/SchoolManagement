using CourseHub.Application.Common.Interfaces;

namespace CourseHub.Application.Features.Public;

public class PublicCatalogService : IPublicCatalogService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IBatchRepository _batchRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public PublicCatalogService(
        ITeacherRepository teacherRepository,
        ICourseRepository courseRepository,
        IStudentRepository studentRepository,
        IBatchRepository batchRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _teacherRepository = teacherRepository;
        _courseRepository = courseRepository;
        _studentRepository = studentRepository;
        _batchRepository = batchRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<IReadOnlyList<PublicTeacherResponse>> GetPublicTeachersAsync(CancellationToken cancellationToken = default)
    {
        var teachers = await _teacherRepository.GetPublicListAsync(cancellationToken);

        return teachers
            .Select(t => new PublicTeacherResponse(t.Id, t.FirstName, t.LastName, t.ProfileImageUrl, t.Bio))
            .ToList();
    }

    public async Task<IReadOnlyList<PublicCourseResponse>> GetPublicCoursesAsync(CancellationToken cancellationToken = default)
    {
        var courses = await _courseRepository.GetPublicListAsync(cancellationToken);

        return courses
            .Select(c => new PublicCourseResponse(c.Id, c.Name, c.Code, c.Description, c.ThumbnailUrl, c.DurationInMonths))
            .ToList();
    }

    public async Task<InstitutionStatsResponse> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        // NOTE: these are awaited sequentially, not via Task.WhenAll —
        // every repository above shares the same scoped DbContext
        // instance, and EF Core's DbContext is not thread-safe for
        // concurrent operations. Running these in parallel would throw
        // "A second operation was started on this context instance
        // before a previous operation completed." at runtime. Five
        // lightweight COUNT queries sequentially is still fast; if this
        // ever needs true concurrency, use IDbContextFactory to give each
        // count its own DbContext instance.
        var totalTeachers = await _teacherRepository.CountActiveAsync(cancellationToken);
        var totalStudents = await _studentRepository.CountActiveAsync(cancellationToken);
        var totalCourses = await _courseRepository.CountActiveAsync(cancellationToken);
        var totalActiveBatches = await _batchRepository.CountActiveAsync(cancellationToken);
        var totalEnrollments = await _enrollmentRepository.CountActiveOrCompletedAsync(cancellationToken);

        return new InstitutionStatsResponse(
            totalTeachers,
            totalStudents,
            totalCourses,
            totalActiveBatches,
            totalEnrollments);
    }
}
