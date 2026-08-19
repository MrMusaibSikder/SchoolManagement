namespace CourseHub.Application.Features.Teachers.Dtos;

/// <summary>
/// ProfileImageUrl is stored and returned exactly as given — a path/URL
/// to wherever the image actually lives. CourseHub does not host or
/// process image files itself (same convention as
/// Courses.UpdateCourseThumbnailRequest).
/// </summary>
public record UpdateTeacherProfileImageRequest(string? ProfileImageUrl);
