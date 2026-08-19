namespace CourseHub.Application.Features.Auth.Dtos;

/// <summary>
/// CourseHub is single-institute, so there's no InstitutionId here.
/// - RequestedRole: optional, "Teacher" or "Student" (case-insensitive).
///   Defaults to "Student" when omitted. "Admin" cannot be self-requested
///   here — admin role assignment is a future, SuperAdmin-only action
///   (Phase 9's role management), to avoid privilege escalation via a
///   public endpoint.
/// - SuperAdminCode: optional. If it matches the configured
///   Seed:SuperAdminInviteCode, the account is granted SuperAdmin instead
///   of RequestedRole. Anyone who doesn't know the code simply falls back
///   to the normal Teacher/Student path.
/// </summary>
public record RegisterRequest(
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName,
    string? RequestedRole = null,
    string? SuperAdminCode = null);
