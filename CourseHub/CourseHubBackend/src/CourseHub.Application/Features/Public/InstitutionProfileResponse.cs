namespace CourseHub.Application.Features.Public;

/// <summary>
/// Public-facing landing-page data for the single institute. No
/// authentication required to read this.
/// </summary>
public record InstitutionProfileResponse(
    string Name,
    string Slug,
    string? LogoUrl,
    string? CoverImageUrl,
    string? Description,
    string? Address,
    string? Phone,
    string? Email,
    string? Website);
