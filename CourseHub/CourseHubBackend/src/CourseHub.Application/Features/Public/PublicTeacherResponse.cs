namespace CourseHub.Application.Features.Public;

/// <summary>
/// Public-facing teacher card — deliberately excludes Phone/Email even
/// though Teacher stores them, since this is an unauthenticated endpoint.
/// ProfileImageUrl is the image path stored on the Teacher row in the
/// database (see Teacher.ProfileImageUrl / Teacher.UpdateProfileImage) —
/// returned as-is; CourseHub does not currently host images itself, so
/// this is expected to be a full URL (e.g. to cloud storage) once Phase
/// 12's admin Teachers CRUD adds an UpdateProfileImage endpoint.
/// </summary>
public record PublicTeacherResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string? ProfileImageUrl,
    string? Bio);
