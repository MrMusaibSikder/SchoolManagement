namespace CourseHub.Application.Features.Users.Dtos;

/// <summary>
/// "SuperAdmin" is deliberately rejected here (see UserManagementService.
/// AssignRoleAsync) — it can only ever be granted through the invite-code
/// bootstrap flow in AuthenticationService.RegisterAsync, never through
/// general role management. Keeps "who can become SuperAdmin" to exactly
/// one, deliberate code path.
/// </summary>
public record AssignRoleRequest(string RoleName);
