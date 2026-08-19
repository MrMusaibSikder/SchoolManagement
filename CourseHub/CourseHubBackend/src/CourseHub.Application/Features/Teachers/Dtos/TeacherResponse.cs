namespace CourseHub.Application.Features.Teachers.Dtos;

public record TeacherResponse(
    Guid Id,
    Guid UserId,
    string EmployeeId,
    string FirstName,
    string LastName,
    string? ProfileImageUrl,
    string? Phone,
    string? Email,
    string? Bio,
    bool IsActive,
    bool IsProfilePublic,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
