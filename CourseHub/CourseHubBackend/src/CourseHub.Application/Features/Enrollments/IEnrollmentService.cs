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
    /// Pending/Active -> Cancelled. This is also what the controller's
    /// DELETE endpoint calls — Enrollment has no soft-delete/IsActive
    /// flag of its own; Cancelled is its terminal, non-occupying state,
    /// so there is nothing more for a "delete" to do beyond this.
    /// </summary>
    Task<EnrollmentResponse> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
