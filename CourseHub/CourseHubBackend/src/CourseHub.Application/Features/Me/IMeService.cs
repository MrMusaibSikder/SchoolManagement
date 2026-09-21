using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Features.Assignments.Dtos;
using CourseHub.Application.Features.Enrollments.Dtos;

namespace CourseHub.Application.Features.Me;

/// <summary>
/// Self-service endpoints for the currently authenticated user — as
/// opposed to IAuthenticationService.GetCurrentUserAsync (identity/
/// profile info) or the admin-facing IEnrollmentService (manages any
/// student's enrollments). This only ever looks up data belonging to the
/// caller themselves; there is no id parameter anywhere here on purpose.
/// </summary>
public interface IMeService
{
    /// <summary>
    /// The caller's own enrollments. Throws NotFoundException if the
    /// caller doesn't have a Student profile yet (e.g. a Teacher/Admin
    /// calling this, or a Student-role user who hasn't been promoted to
    /// a Student profile by an admin yet).
    /// </summary>
    Task<PagedResult<EnrollmentResponse>> GetMyEnrollmentsAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MyAssignmentResponse>> GetMyAssignmentsAsync(
       Guid userId,
       CancellationToken cancellationToken = default);

    Task<PagedResult<SubmissionResponse>> GetMySubmissionsAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
