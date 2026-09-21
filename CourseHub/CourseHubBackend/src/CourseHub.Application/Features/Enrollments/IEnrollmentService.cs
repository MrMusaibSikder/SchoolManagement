using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Features.Enrollments.Dtos;
using CourseHub.Domain.Enums;

namespace CourseHub.Application.Features.Enrollments;

public interface IEnrollmentService
{
    Task<PagedResult<EnrollmentResponse>> SearchAsync(
        Guid? studentId,
        Guid? batchId,
        EnrollmentStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<EnrollmentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<EnrollmentResponse> CreateAsync(CreateEnrollmentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pending -> Active.
    /// </summary>
    Task<EnrollmentResponse> ApproveAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Active -> Completed.
    /// </summary>
    Task<EnrollmentResponse> CompleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pending/Active -> Cancelled.
    /// </summary>
    Task<EnrollmentResponse> CancelAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently removes the enrollment row. Only allowed once the
    /// enrollment is Cancelled or Completed — DELETE on a Pending/Active
    /// enrollment fails; cancel it first.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
