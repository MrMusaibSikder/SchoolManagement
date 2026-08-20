namespace CourseHub.Application.Features.Enrollments.Dtos;

/// <summary>
/// Enrolls an existing Student into an existing Batch. Starts life as
/// Status=Pending (see Enrollment.Create) — use the /approve endpoint to
/// move it to Active. See EnrollmentService.CreateAsync for the full
/// validation chain (student must exist and be active, batch must exist
/// and be active, no duplicate enrollment for the same pair, and the
/// batch must have a free seat if it has a capacity limit).
/// </summary>
public record CreateEnrollmentRequest(Guid StudentId, Guid BatchId);
