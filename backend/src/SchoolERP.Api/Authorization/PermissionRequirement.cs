using Microsoft.AspNetCore.Authorization;

namespace SchoolERP.Api.Authorization;

/// <summary>
/// An <see cref="IAuthorizationRequirement"/> that is satisfied only when the
/// current user has the named permission (see <see cref="SchoolERP.Domain.Constants.PermissionNames"/>)
/// through one of their assigned roles.
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>The permission name (e.g. "Student.Edit") that must be granted.</summary>
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
