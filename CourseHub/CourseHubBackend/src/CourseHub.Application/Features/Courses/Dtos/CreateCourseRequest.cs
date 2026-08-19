namespace CourseHub.Application.Features.Courses.Dtos;

public record CreateCourseRequest(string Name, string Code, int DurationInMonths, string? Description);
