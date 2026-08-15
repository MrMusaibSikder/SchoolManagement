using CourseHub.Domain.Common;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Domain.Entities;

/// <summary>
/// RolePermission is the many-to-many join between Role and Permission.
/// A role gains its effective capabilities through this relationship.
/// </summary>
public class RolePermission : BaseEntity
{
    public Guid RoleId { get; private set; }

    public Guid PermissionId { get; private set; }

    private RolePermission()
    {
    }

    private RolePermission(Guid roleId, Guid permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public static RolePermission Create(Guid roleId, Guid permissionId)
    {
        if (roleId == Guid.Empty)
        {
            throw new ValidationException("RoleId is required.");
        }

        if (permissionId == Guid.Empty)
        {
            throw new ValidationException("PermissionId is required.");
        }

        return new RolePermission(roleId, permissionId);
    }
}
