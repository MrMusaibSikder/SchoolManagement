using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;

namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Phase 11 added the read-only aggregate-count slice
/// (CountActiveOrCompletedAsync). Phase 12 extends it here with the full
/// set needed by the admin Enrollments CRUD.
/// </summary>
public interface IEnrollmentRepository
{
    /// <summary>
    /// Count of Active + Completed enrollments — i.e. "students who have
    /// ever actually joined a batch", excluding Pending (not yet
    /// approved) and Cancelled.
    /// </summary>
    Task<int> CountActiveOrCompletedAsync(CancellationToken cancellationToken = default);

    Task<Enrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// (StudentId, BatchId) has a unique DB index (see
    /// EnrollmentConfiguration) — a student cannot enroll in the same
    /// batch twice.
    /// </summary>
    Task<bool> ExistsForStudentAndBatchAsync(Guid studentId, Guid batchId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Count of enrollments in a batch matching any of the given
    /// statuses — used to enforce Batch.Capacity (Pending + Active count
    /// against the limit; Cancelled/Completed don't occupy a seat).
    /// </summary>
    Task<int> CountForBatchByStatusesAsync(Guid batchId, IReadOnlyList<EnrollmentStatus> statuses, CancellationToken cancellationToken = default);

    /// <summary>
    /// Admin enrollment listing: optionally filtered by student, batch,
    /// and/or status, paged.
    /// </summary>
    Task<(IReadOnlyList<Enrollment> Items, int TotalCount)> SearchAsync(
        Guid? studentId,
        Guid? batchId,
        EnrollmentStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Enrollment>> GetActiveByBatchIdAsync(
    Guid batchId,
    CancellationToken cancellationToken = default);

    Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hard-deletes an enrollment row. EnrollmentService only calls this
    /// for Cancelled/Completed enrollments — Pending/Active ones must be
    /// cancelled first, so this never silently discards an in-progress
    /// enrollment.
    /// </summary>
    Task RemoveAsync(Enrollment enrollment, CancellationToken cancellationToken = default);


    /// <summary>
    /// Distinct student ids actively/completed-enrolled in any batch under
    /// the given course — the grading roster's base list ("who's supposed
    /// to submit this"), independent of which specific batch they're in.
    /// </summary>
    Task<IReadOnlyList<Guid>> GetEnrolledStudentIdsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<bool> ExistsForStudentAndCourseAsync(
    Guid studentId,
    Guid courseId,
    CancellationToken cancellationToken = default);

    

    Task<IReadOnlyList<Guid>> GetEnrolledCourseIdsByStudentIdAsync(
    Guid studentId,
    CancellationToken cancellationToken = default);
}
