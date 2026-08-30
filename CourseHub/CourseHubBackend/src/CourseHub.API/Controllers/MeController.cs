using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Enrollments.Dtos;
using CourseHub.Application.Features.Me;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers;

/// <summary>
/// Self-service endpoints — "show me my own data". Only requires being
/// authenticated ([Authorize] with no policy/permission), unlike every
/// admin controller which requires a specific permission — a Student
/// doesn't need any special grant to see their own enrollments, the same
/// way any logged-in user can already call GET /api/auth/me.
/// </summary>
[Route("api/me")]
[Authorize]
public class MeController : ApiControllerBase
{
    private readonly IMeService _meService;
    private readonly ICurrentUserService _currentUserService;

    public MeController(IMeService meService, ICurrentUserService currentUserService)
    {
        _meService = meService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// The logged-in student's own enrollments. 404 if the caller doesn't
    /// have a student profile yet (e.g. a Teacher/Admin account, or a
    /// Student-role user an admin hasn't promoted into a profile yet).
    /// </summary>
    [HttpGet("enrollments")]
    [ProducesResponseType(typeof(PagedResult<EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<EnrollmentResponse>>> GetMyEnrollments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        // ICurrentUserService.UserId is always set here — [Authorize]
        // above guarantees a valid JWT with a "sub" claim reached this
        // point at all.
        var userId = _currentUserService.UserId!.Value;

        var result = await _meService.GetMyEnrollmentsAsync(userId, page, pageSize, cancellationToken);
        return Ok(result);
    }
}
