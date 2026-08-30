namespace CourseHub.Application.Features.Users.Dtos;

/// <summary>
/// A user who holds the right role but doesn't have a Teacher/Student
/// profile yet — i.e. a valid candidate to promote. Deliberately minimal
/// (no phone/status/etc.) since this only exists to populate an admin
/// "pick a user" dropdown.
/// </summary>
public record EligibleUserResponse(Guid Id, string Email, string FirstName, string LastName);
