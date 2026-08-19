namespace CourseHub.Application.Features.Students.Dtos;

public record StudentResponse(
    Guid Id,
    Guid UserId,
    string StudentId,
    string FirstName,
    string LastName,
    string? ProfileImageUrl,
    DateTime? DateOfBirth,
    string? Phone,
    string? Email,
    string? Address,
    string? GuardianName,
    string? GuardianPhone,
    bool IsActive,
    bool IsProfilePublic,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
