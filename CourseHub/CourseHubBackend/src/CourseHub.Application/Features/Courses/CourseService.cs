using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Courses.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Application.Features.Courses;

public class CourseService : ICourseService
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<CourseResponse>> SearchAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        // Never trust page/pageSize from the query string as-is — a
        // caller passing page=0 or pageSize=100000 must not be able to
        // force a full-table scan or a negative Skip().
        var normalizedPage = Math.Max(page, 1);
        var normalizedPageSize = Math.Clamp(pageSize <= 0 ? DefaultPageSize : pageSize, 1, MaxPageSize);

        var (items, totalCount) = await _courseRepository.SearchAsync(search, normalizedPage, normalizedPageSize, cancellationToken);

        var responses = items.Select(ToResponse).ToList();

        return new PagedResult<CourseResponse>(responses, totalCount, normalizedPage, normalizedPageSize);
    }

    public async Task<CourseResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await RequireCourseAsync(id, cancellationToken);
        return ToResponse(course);
    }

    public async Task<CourseResponse> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureCodeIsAvailableAsync(request.Code, excludingId: null, cancellationToken);

        var course = Course.Create(request.Name, request.Code, request.DurationInMonths, request.Description);

        await _courseRepository.AddAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(course);
    }

    public async Task<CourseResponse> UpdateAsync(Guid id, UpdateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var course = await RequireCourseAsync(id, cancellationToken);

        await EnsureCodeIsAvailableAsync(request.Code, excludingId: course.Id, cancellationToken);

        course.Update(request.Name, request.Code, request.Description, request.DurationInMonths);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(course);
    }

    public async Task<CourseResponse> UpdateThumbnailAsync(Guid id, UpdateCourseThumbnailRequest request, CancellationToken cancellationToken = default)
    {
        var course = await RequireCourseAsync(id, cancellationToken);

        course.UpdateThumbnail(request.ThumbnailUrl);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(course);
    }

    public async Task<CourseResponse> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await RequireCourseAsync(id, cancellationToken);
        course.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(course);
    }

    public async Task<CourseResponse> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await RequireCourseAsync(id, cancellationToken);
        course.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(course);
    }

    public async Task<CourseResponse> PublishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await RequireCourseAsync(id, cancellationToken);
        course.MakePublic();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(course);
    }

    public async Task<CourseResponse> UnpublishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await RequireCourseAsync(id, cancellationToken);
        course.MakePrivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(course);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await RequireCourseAsync(id, cancellationToken);

        // Deliberately a soft delete (Deactivate), never a hard row
        // delete: Batch.CourseId has DeleteBehavior.Restrict against
        // Course (see BatchConfiguration), so hard-deleting a course that
        // has any batches would fail with a raw FK-constraint
        // DbUpdateException — a confusing 500 instead of a clean
        // response. Deactivating keeps the row (and every batch's/
        // enrollment's history) intact and simply hides the course from
        // the public catalog (GetPublicListAsync filters on IsActive).
        course.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureCodeIsAvailableAsync(string code, Guid? excludingId, CancellationToken cancellationToken)
    {
        var codeTaken = await _courseRepository.ExistsByCodeAsync(code, excludingId, cancellationToken);

        if (codeTaken)
        {
            throw new ValidationException($"A course with code '{code}' already exists.");
        }
    }

    private async Task<Course> RequireCourseAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _courseRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Course", id);
    }

    private static CourseResponse ToResponse(Course course) => new(
        course.Id,
        course.Name,
        course.Code,
        course.Description,
        course.ThumbnailUrl,
        course.DurationInMonths,
        course.IsActive,
        course.IsPublic,
        course.CreatedAt,
        course.UpdatedAt);
}
