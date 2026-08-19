namespace CourseHub.Application.Features.Students.Dtos;

public record UpdateStudentProfileRequest(string FirstName, string LastName, DateTime? DateOfBirth);
