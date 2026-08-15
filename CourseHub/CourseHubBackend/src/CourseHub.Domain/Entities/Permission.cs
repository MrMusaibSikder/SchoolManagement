using CourseHub.Domain.Common;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Domain.Entities;

/// <summary>
/// Permission is a reusable, global capability definition (e.g. Resource
/// "Course", Action "Create"). It is not tied to a specific User or
/// Institution — Roles gain permissions through RolePermission.
/// </summary>
public class Permission : BaseEntity
{
    /// <summary>
    /// Unique machine name, e.g. "courses.create".
    /// </summary>
    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    /// <summary>
    /// The capability's target, e.g. "Course", "Student".
    /// </summary>
    public string Resource { get; private set; } = null!;

    /// <summary>
    /// The action performed on the resource, e.g. "Create", "View".
    /// </summary>
    public string Action { get; private set; } = null!;

    private Permission()
    {
    }

    private Permission(string name, string resource, string action, string? description)
    {
        Name = name;
        Resource = resource;
        Action = action;
        Description = description;
    }

    public static Permission Create(string name, string resource, string action, string? description = null)
    {
        var validatedName = ValidateRequired(name, nameof(name));
        var validatedResource = ValidateRequired(resource, nameof(resource));
        var validatedAction = ValidateRequired(action, nameof(action));

        return new Permission(validatedName, validatedResource, validatedAction, description);
    }

    public void Update(string description)
    {
        Description = description;
        MarkAsUpdated();
    }

    private static string ValidateRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException($"{fieldName} is required.");
        }

        return value.Trim();
    }
}
