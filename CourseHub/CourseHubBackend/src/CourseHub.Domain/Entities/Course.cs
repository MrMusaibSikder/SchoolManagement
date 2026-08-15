using CourseHub.Domain.Common;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Domain.Entities;

/// <summary>
/// Course represents a course/program offered by an Institution.
/// A Course may have multiple running Batches (cohorts).
/// </summary>
public class Course : BaseEntity
{
    public Guid InstitutionId { get; private set; }

    public string Name { get; private set; } = null!;

    /// <summary>
    /// Institution-level business identifier, e.g. "CS101".
    /// </summary>
    public string Code { get; private set; } = null!;

    public string? Description { get; private set; }

    public string? ThumbnailUrl { get; private set; }

    public int DurationInMonths { get; private set; }

    public bool IsActive { get; private set; }

    public bool IsPublic { get; private set; }

    private Course()
    {
    }

    private Course(Guid institutionId, string name, string code, string? description, int durationInMonths)
    {
        InstitutionId = institutionId;
        Name = name;
        Code = code;
        Description = description;
        DurationInMonths = durationInMonths;
        IsActive = true;
        IsPublic = false;
    }

    public static Course Create(
        Guid institutionId,
        string name,
        string code,
        int durationInMonths,
        string? description = null)
    {
        if (institutionId == Guid.Empty)
        {
            throw new ValidationException("InstitutionId is required.");
        }

        var validatedName = ValidateRequired(name, "Name");
        var validatedCode = ValidateRequired(code, "Code");
        var validatedDuration = ValidateDuration(durationInMonths);

        return new Course(institutionId, validatedName, validatedCode, description, validatedDuration);
    }

    public void Update(string name, string code, string? description, int durationInMonths)
    {
        Name = ValidateRequired(name, "Name");
        Code = ValidateRequired(code, "Code");
        Description = description;
        DurationInMonths = ValidateDuration(durationInMonths);
        MarkAsUpdated();
    }

    public void UpdateThumbnail(string? thumbnailUrl)
    {
        ThumbnailUrl = thumbnailUrl;
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

    public void MakePublic()
    {
        IsPublic = true;
        MarkAsUpdated();
    }

    public void MakePrivate()
    {
        IsPublic = false;
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

    private static int ValidateDuration(int durationInMonths)
    {
        if (durationInMonths <= 0)
        {
            throw new ValidationException("DurationInMonths must be a positive integer.");
        }

        return durationInMonths;
    }
}
