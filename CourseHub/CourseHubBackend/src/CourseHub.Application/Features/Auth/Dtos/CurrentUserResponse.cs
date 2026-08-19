namespace CourseHub.Application.Features.Auth.Dtos;

public record CurrentUserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Status,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions,
    DateTime? LastLoginAt);
