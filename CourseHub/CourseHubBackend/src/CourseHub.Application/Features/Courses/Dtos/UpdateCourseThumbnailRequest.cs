namespace CourseHub.Application.Features.Courses.Dtos;

/// <summary>
/// ThumbnailUrl is stored and returned exactly as given — a path/URL to
/// wherever the image actually lives (e.g. cloud storage). CourseHub does
/// not host or process image files itself.
/// </summary>
public record UpdateCourseThumbnailRequest(string? ThumbnailUrl);
