using Microsoft.AspNetCore.Authorization;

namespace SchoolERP.Api.Authorization;

/// <summary>
/// Custom permission-based authorization attribute. Usage:
/// <c>[PermissionAuthorize(PermissionNames.StudentEdit)]</c>.
/// Internally maps to a dynamically created ASP.NET Core authorization policy
/// named "Permission:{permission}", resolved at runtime by
/// <see cref="PermissionPolicyProvider"/> and enforced by <see cref="PermissionHandler"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class PermissionAuthorizeAttribute : AuthorizeAttribute
{
    /// <summary>Prefix used to distinguish permission policies from any other named policy.</summary>
    public const string PolicyPrefix = "Permission:";

    public PermissionAuthorizeAttribute(string permission) : base(PolicyPrefix + permission)
    {
    }
}
