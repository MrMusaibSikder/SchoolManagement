namespace CourseHub.Application.Features.Enrollments.Dtos;

public record EnrollmentResponse(
    Guid Id,
    Guid StudentId,
    Guid BatchId,
    DateTime EnrollmentDate,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
