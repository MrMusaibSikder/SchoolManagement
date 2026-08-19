using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Features.Teachers.Dtos;

namespace CourseHub.Application.Features.Teachers;

public interface ITeacherService
{
    Task<PagedResult<TeacherResponse>> SearchAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<TeacherResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TeacherResponse> CreateAsync(CreateTeacherRequest request, CancellationToken cancellationToken = default);

    Task<TeacherResponse> UpdateProfileAsync(Guid id, UpdateTeacherProfileRequest request, CancellationToken cancellationToken = default);

    Task<TeacherResponse> UpdateContactAsync(Guid id, UpdateTeacherContactRequest request, CancellationToken cancellationToken = default);

    Task<TeacherResponse> UpdateProfileImageAsync(Guid id, UpdateTeacherProfileImageRequest request, CancellationToken cancellationToken = default);

    Task<TeacherResponse> ActivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TeacherResponse> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TeacherResponse> MakeProfilePublicAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TeacherResponse> MakeProfilePrivateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-delete: deactivates the teacher rather than removing the row.
    /// See TeacherService.DeleteAsync for why.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
