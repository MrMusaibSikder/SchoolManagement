using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Features.Courses.Dtos;

namespace CourseHub.Application.Features.Courses;

public interface ICourseService
{
    Task<PagedResult<CourseResponse>> SearchAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<CourseResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CourseResponse> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken = default);

    Task<CourseResponse> UpdateAsync(Guid id, UpdateCourseRequest request, CancellationToken cancellationToken = default);

    Task<CourseResponse> UpdateThumbnailAsync(Guid id, UpdateCourseThumbnailRequest request, CancellationToken cancellationToken = default);

    Task<CourseResponse> ActivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CourseResponse> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CourseResponse> PublishAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CourseResponse> UnpublishAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-delete: deactivates the course rather than removing the row.
    /// See CourseService.DeleteAsync for why a hard delete is never done.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
