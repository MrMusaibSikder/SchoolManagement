using CourseHub.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;

namespace CourseHub.API.Security;

/// <summary>
/// Evaluates a PermissionRequirement against the current user's claims.
///
/// SuperAdmin's real source of authority is the explicit RolePermission
/// rows seeded by DatabaseSeeder — SuperAdmin is auto-linked to every
/// permission in the catalog on every startup, so its permissions are
/// visible in the DB/admin UI like any other role's.
///
/// The IsInRole(SuperAdmin) check below is a safety net on top of that,
/// not the primary mechanism: it only matters in the narrow window where
/// a brand-new Permission has been created (e.g. via a future "create
/// permission" admin endpoint) but the API hasn't restarted yet to run
/// seeding and link it to SuperAdmin. Cheap to keep, and it can be
/// removed later without changing behavior once permissions are only
/// ever added through the seeded catalog.
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
