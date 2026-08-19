namespace CourseHub.Application.Features.Students.Dtos;

/// <summary>
/// Creates a Student profile for an EXISTING user account — same
/// "promotion, not standalone creation" pattern as
/// Teachers.CreateTeacherRequest. See StudentService.CreateAsync for the
/// full validation chain (user must exist, must actually hold the
/// Student role, must not already have a profile, StudentId must be
/// unique).
/// </summary>
public record CreateStudentRequest(Guid UserId, string StudentId, string FirstName, string LastName);
