namespace CourseHub.Application.Features.Teachers.Dtos;

/// <summary>
/// Creates a Teacher profile for an EXISTING user account — Teacher
/// profiles are never created standalone. The admin picks a user who
/// already registered (typically with requestedRole "Teacher") and
/// promotes them by supplying an EmployeeId + display name. See
/// TeacherService.CreateAsync for the full validation chain (user must
/// exist, must actually hold the Teacher role, must not already have a
/// profile).
/// </summary>
public record CreateTeacherRequest(Guid UserId, string EmployeeId, string FirstName, string LastName);
