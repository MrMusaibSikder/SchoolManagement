namespace CourseHub.Application.Features.Users.Dtos;

public record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Status,
    IReadOnlyList<string> Roles,
    DateTime? LastLoginAt,
    DateTime CreatedAt);
