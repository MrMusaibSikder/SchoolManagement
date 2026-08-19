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

    public PublicController(IPublicInstitutionService publicInstitutionService)
    {
        _publicInstitutionService = publicInstitutionService;
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
}
