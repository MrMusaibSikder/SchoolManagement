using CourseHub.Domain.Common;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Domain.Entities;

/// <summary>
/// Role is a dynamic, database-driven authorization grouping.
/// Institution admins create roles at runtime — there is no hard-coded
/// Role enum. A Role gains capabilities only through RolePermission.
/// </summary>
public class Role : BaseEntity
{
    /// <summary>
    /// Owning institution. Null for platform-wide system roles
    /// (see IsSystemRole); always set for institution-created roles.
    /// </summary>
    public Guid? InstitutionId { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public bool IsSystemRole { get; private set; }

    private Role()
    {
    }

    private Role(Guid? institutionId, string name, string? description, bool isSystemRole)
    {
        InstitutionId = institutionId;
        Name = name;
        Description = description;
        IsSystemRole = isSystemRole;
        IsActive = true;
    }

    /// <summary>
    /// Creates an institution-owned role (the normal case — e.g. an
    /// Institution Admin defining a "Course Coordinator" role).
    /// </summary>
    public static Role Create(Guid institutionId, string name, string? description = null)
    {
        if (institutionId == Guid.Empty)
        {
            throw new ValidationException("InstitutionId is required for an institution role.");
        }

        var validatedName = ValidateName(name);
        return new Role(institutionId, validatedName, description, isSystemRole: false);
    }

    /// <summary>
    /// Creates a platform-wide system role with no institution owner
    /// (e.g. Super Admin). Reserved for platform provisioning/seeding;
    /// not exposed through normal role-management flows.
    /// </summary>
    public static Role CreateSystemRole(string name, string? description = null)
    {
        var validatedName = ValidateName(name);
        return new Role(null, validatedName, description, isSystemRole: true);
    }

    public void Update(string name, string? description)
    {
        Name = ValidateName(name);
        Description = description;
        MarkAsUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Role name is required.");
        }

        return name.Trim();
    }
}
