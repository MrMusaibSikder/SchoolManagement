using Microsoft.AspNetCore.Authorization;

namespace CourseHub.API.Extensions;

/// <summary>
/// Registers Phase 9's permission-based authorization pieces. Kept
/// separate from AuthenticationExtensions (which configures JWT bearer
/// authentication itself) since authentication ("who are you") and
/// authorization ("what can you do") are distinct concerns — call both
/// from Program.cs:
/// builder.Services.AddApiAuthentication(builder.Configuration);
/// builder.Services.AddApiAuthorization();
/// </summary>
public static class AuthorizationExtensions
{
    public static IServiceCollection AddApiAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();

        // Replaces the default policy provider so any [HasPermission("x.y")]
        // / [Authorize(Policy = "x.y")] resolves to a PermissionRequirement
        // automatically — see PermissionPolicyProvider for why this scales
        // better than registering one AddPolicy(...) call per permission.
        services.AddSingleton<IAuthorizationPolicyProvider, CourseHub.API.Security.PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, CourseHub.API.Security.PermissionAuthorizationHandler>();

        return services;
    }
}
