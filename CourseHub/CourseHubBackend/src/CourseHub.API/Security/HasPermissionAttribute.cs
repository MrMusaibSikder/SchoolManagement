using Microsoft.AspNetCore.Authorization;

namespace CourseHub.API.Security;

/// <summary>
/// Syntactic sugar over [Authorize(Policy = "...")] so controllers read
/// declaratively: [HasPermission("courses.create")] instead of a raw
/// magic-string Policy. Resolved at runtime by PermissionPolicyProvider —
/// no registration needed anywhere else.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
        : base(policy: permission)
    {
    }
}
