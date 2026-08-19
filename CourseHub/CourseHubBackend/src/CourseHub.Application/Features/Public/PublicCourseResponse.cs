namespace CourseHub.Application.Features.Public;

/// <summary>
/// Public-facing course card. ThumbnailUrl is the image path stored on
/// the Course row (see Course.ThumbnailUrl / Course.UpdateThumbnail) —
/// returned as-is.
/// </summary>
public record PublicCourseResponse(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    string? ThumbnailUrl,
    int DurationInMonths);
