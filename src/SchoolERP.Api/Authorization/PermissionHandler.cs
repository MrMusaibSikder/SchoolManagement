using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SchoolERP.Application.Features.Authorization.Interfaces;

namespace SchoolERP.Api.Authorization;

/// <summary>
/// Evaluates <see cref="PermissionRequirement"/>s by looking up the current
/// user's id from their claims and asking <see cref="IUserAccessService"/>
/// whether that user has the required permission through any assigned role.
/// </summary>
public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUserAccessService _userAccessService;

    public PermissionHandler(IUserAccessService userAccessService)
    {
        _userAccessService = userAccessService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        var hasPermission = await _userAccessService.HasPermissionAsync(userId, requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
