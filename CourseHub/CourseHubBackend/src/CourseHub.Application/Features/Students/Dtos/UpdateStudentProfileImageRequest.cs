namespace CourseHub.Application.Features.Students.Dtos;

/// <summary>
/// ProfileImageUrl is stored and returned exactly as given — same
/// convention as Teachers/Courses image fields.
/// </summary>
public record UpdateStudentProfileImageRequest(string? ProfileImageUrl);
