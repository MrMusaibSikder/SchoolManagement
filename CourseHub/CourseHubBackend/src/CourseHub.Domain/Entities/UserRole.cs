using CourseHub.Domain.Common;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Domain.Entities;

/// <summary>
/// UserRole is the many-to-many join between User and Role.
/// A user may hold multiple roles simultaneously.
/// </summary>
public class UserRole : BaseEntity
{
    public Guid UserId { get; private set; }

    public Guid RoleId { get; private set; }

    private UserRole()
    {
    }

    private UserRole(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public static UserRole Create(Guid userId, Guid roleId)
    {
        if (userId == Guid.Empty)
        {
            throw new ValidationException("UserId is required.");
        }

        if (roleId == Guid.Empty)
        {
            throw new ValidationException("RoleId is required.");
        }

        return new UserRole(userId, roleId);
    }
}
