namespace CourseHub.Application.Common.Security;

/// <summary>
/// Single source of truth for the custom JWT claim type used to carry a
/// user's effective permissions (derived from their roles at token-issue
/// time). Shared between Infrastructure (JwtTokenService, which writes the
/// claim) and API (PermissionAuthorizationHandler/CurrentUserService,
/// which read it) so both sides can never drift out of sync.
/// </summary>
public static class PermissionClaimTypes
{
    public const string Permission = "permission";
}
