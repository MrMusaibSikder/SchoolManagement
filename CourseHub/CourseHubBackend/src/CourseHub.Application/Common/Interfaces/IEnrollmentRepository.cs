namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Minimal, read-only slice needed by Phase 11's public stats endpoint.
/// Phase 12 will extend this with the admin Enrollments CRUD.
/// </summary>
public interface IEnrollmentRepository
{
    /// <summary>
    /// Count of Active + Completed enrollments — i.e. "students who have
    /// ever actually joined a batch", excluding Pending (not yet
    /// approved) and Cancelled.
    /// </summary>
    Task<int> CountActiveOrCompletedAsync(CancellationToken cancellationToken = default);
}
