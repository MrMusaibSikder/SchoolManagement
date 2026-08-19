using CourseHub.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;

namespace CourseHub.API.Security;

/// <summary>
/// Evaluates a PermissionRequirement against the current user's claims.
///
/// SuperAdmin always succeeds, regardless of which permissions are
/// actually seeded/assigned in RolePermission — this is a deliberate
/// bypass (not a seeded "has every permission" row) so the SuperAdmin
/// role never needs updating every time a later phase adds a new
/// permission (see SeedOptions.DefaultRolePermissions).
///
/// Everyone else must have the exact permission name present as a
/// "permission" claim on their JWT (baked in at login/refresh time by
/// JwtTokenService — see IJwtTokenService for the staleness trade-off).
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.IsInRole(SystemRoleNames.SuperAdmin))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var hasPermission = context.User.Claims.Any(claim =>
            claim.Type == PermissionClaimTypes.Permission &&
            string.Equals(claim.Value, requirement.Permission, StringComparison.OrdinalIgnoreCase));

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
