using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Features.Students.Dtos;

namespace CourseHub.Application.Features.Students;

public interface IStudentService
{
    Task<PagedResult<StudentResponse>> SearchAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<StudentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);

    Task<StudentResponse> UpdateProfileAsync(Guid id, UpdateStudentProfileRequest request, CancellationToken cancellationToken = default);

    Task<StudentResponse> UpdateContactAsync(Guid id, UpdateStudentContactRequest request, CancellationToken cancellationToken = default);

    Task<StudentResponse> UpdateGuardianAsync(Guid id, UpdateStudentGuardianRequest request, CancellationToken cancellationToken = default);

    Task<StudentResponse> UpdateProfileImageAsync(Guid id, UpdateStudentProfileImageRequest request, CancellationToken cancellationToken = default);

    Task<StudentResponse> ActivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<StudentResponse> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<StudentResponse> MakeProfilePublicAsync(Guid id, CancellationToken cancellationToken = default);

    Task<StudentResponse> MakeProfilePrivateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-delete: deactivates the student rather than removing the row.
    /// See StudentService.DeleteAsync for why.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
