using Microsoft.AspNetCore.Authorization;

namespace CourseHub.API.Security;

/// <summary>
/// Generates an AuthorizationPolicy on demand for any policy name used
/// with [HasPermission("...")] / [Authorize(Policy = "...")], instead of
/// requiring every permission to be pre-registered with AddPolicy(...) in
/// Program.cs. This is what lets Phase 12 add dozens of new permissions
/// (courses.create, students.manage, etc.) without ever touching
/// Program.cs again — the policy is just the permission name, and this
/// provider turns any unrecognized policy name into a PermissionRequirement.
///
/// Falls back to the default provider for the few named policies that
/// aren't permission-based (currently none, but this keeps the door open
/// without special-casing).
/// </summary>
public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public PermissionPolicyProvider(Microsoft.Extensions.Options.IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackPolicyProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (string.IsNullOrWhiteSpace(policyName) || !policyName.Contains('.'))
        {
            // Permission names in this codebase are always
            // "resource.action" (see SeedOptions.DefaultPermissions), so a
            // policy name without a dot can never be one — defer to the
            // default provider (statically registered policies, if any).
            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}
