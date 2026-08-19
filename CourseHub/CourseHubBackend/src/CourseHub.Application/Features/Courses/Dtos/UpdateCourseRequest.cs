namespace CourseHub.Application.Features.Courses.Dtos;

public record UpdateCourseRequest(string Name, string Code, int DurationInMonths, string? Description);
