using Microsoft.AspNetCore.Authorization;

namespace CourseHub.API.Security;

/// <summary>
/// One requirement instance per permission name (e.g. "courses.create").
/// Created on demand by PermissionPolicyProvider rather than registered
/// one-by-one in Program.cs — see that class for why.
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
