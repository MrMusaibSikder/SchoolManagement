namespace CourseHub.Application.Features.Courses.Dtos;

public record CourseResponse(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    string? ThumbnailUrl,
    int DurationInMonths,
    bool IsActive,
    bool IsPublic,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
