using CourseHub.Application.Features.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers;

/// <summary>
/// Unauthenticated, public-facing endpoints. CourseHub is single-institute,
/// so there's exactly one institution profile to expose here.
/// </summary>
[ApiController]
[Route("api/public")]
[AllowAnonymous]
public class PublicController : ControllerBase
{
    private readonly IPublicInstitutionService _publicInstitutionService;
    private readonly IPublicCatalogService _publicCatalogService;

    public PublicController(IPublicInstitutionService publicInstitutionService, IPublicCatalogService publicCatalogService)
    {
        _publicInstitutionService = publicInstitutionService;
        _publicCatalogService = publicCatalogService;
    }

    [HttpGet("institution")]
    [ProducesResponseType(typeof(InstitutionProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InstitutionProfileResponse>> GetInstitution(CancellationToken cancellationToken)
    {
        var profile = await _publicInstitutionService.GetProfileAsync(cancellationToken);

        if (profile is null)
        {
            return NotFound();
        }

        return Ok(profile);
    }

    /// <summary>
    /// Active teachers who have opted their profile into public display.
    /// Each entry includes ProfileImageUrl exactly as stored on the
    /// Teacher row in the database — no phone/email (unauthenticated).
    /// </summary>
    [HttpGet("teachers")]
    [ProducesResponseType(typeof(IReadOnlyList<PublicTeacherResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PublicTeacherResponse>>> GetTeachers(CancellationToken cancellationToken)
    {
        var teachers = await _publicCatalogService.GetPublicTeachersAsync(cancellationToken);
        return Ok(teachers);
    }

    /// <summary>
    /// Active courses the institute has marked public. Each entry
    /// includes ThumbnailUrl exactly as stored on the Course row.
    /// </summary>
    [HttpGet("courses")]
    [ProducesResponseType(typeof(IReadOnlyList<PublicCourseResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PublicCourseResponse>>> GetCourses(CancellationToken cancellationToken)
    {
        var courses = await _publicCatalogService.GetPublicCoursesAsync(cancellationToken);
        return Ok(courses);
    }

    /// <summary>
    /// Aggregate, non-identifying counts (total teachers/students/
    /// courses/active batches/enrollments) — for the landing page's
    /// stats section or a frontend chart. Never exposes which individual
    /// teachers/students/courses exist.
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(InstitutionStatsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<InstitutionStatsResponse>> GetStats(CancellationToken cancellationToken)
    {
        var stats = await _publicCatalogService.GetStatsAsync(cancellationToken);
        return Ok(stats);
    }
}
