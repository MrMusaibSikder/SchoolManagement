using CourseHub.Domain.Common;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Domain.Entities;

/// <summary>
/// Role is a dynamic, database-driven authorization grouping. There is no
/// hard-coded Role enum — an admin can create roles at runtime. CourseHub
/// is single-institute, so Role is no longer institution-scoped;
/// IsSystemRole instead distinguishes seeded, protected roles (SuperAdmin,
/// Admin, Teacher, Student — see Infrastructure/Persistence/Seed) from
/// custom roles an admin creates later.
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public bool IsSystemRole { get; private set; }

    private Role()
    {
    }

    private Role(string name, string? description, bool isSystemRole)
    {
        Name = name;
        Description = description;
        IsSystemRole = isSystemRole;
        IsActive = true;
    }

    /// <summary>
    /// Creates a normal, admin-defined role (e.g. "Course Coordinator").
    /// </summary>
    public static Role Create(string name, string? description = null)
    {
        var validatedName = ValidateName(name);
        return new Role(validatedName, description, isSystemRole: false);
    }

    /// <summary>
    /// Creates a protected, seeded role (SuperAdmin/Admin/Teacher/Student).
    /// Reserved for platform seeding — not exposed through normal
    /// role-management flows.
    /// </summary>
    public static Role CreateSystemRole(string name, string? description = null)
    {
        var validatedName = ValidateName(name);
        return new Role(validatedName, description, isSystemRole: true);
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
