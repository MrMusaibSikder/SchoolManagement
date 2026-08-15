using CourseHub.Domain.Common;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Domain.Entities;

/// <summary>
/// Institution is the tenant/root organization of CourseHub.
/// Every institution-owned entity (User, Teacher, Student, Course, Batch, Enrollment)
/// references an Institution via InstitutionId.
/// </summary>
public class Institution : BaseEntity
{
    public string Name { get; private set; } = null!;

    public string Slug { get; private set; } = null!;

    public string? LogoUrl { get; private set; }

    public string? CoverImageUrl { get; private set; }

    public string? Description { get; private set; }

    public string? Address { get; private set; }

    public string? Phone { get; private set; }

    public string? Email { get; private set; }

    public string? Website { get; private set; }

    public bool IsPublic { get; private set; }

    public bool IsActive { get; private set; }

    // EF Core requires a parameterless constructor; keep it private so
    // callers outside the entity cannot construct an empty/invalid instance.
    private Institution()
    {
    }

    private Institution(
        string name,
        string slug,
        string? description,
        string? logoUrl,
        string? coverImageUrl,
        string? address,
        string? phone,
        string? email,
        string? website,
        bool isPublic)
    {
        Name = name;
        Slug = slug;
        Description = description;
        LogoUrl = logoUrl;
        CoverImageUrl = coverImageUrl;
        Address = address;
        Phone = phone;
        Email = email;
        Website = website;
        IsPublic = isPublic;
        IsActive = true;
    }

    public static Institution Create(
        string name,
        string slug,
        string? description = null,
        string? logoUrl = null,
        string? coverImageUrl = null,
        string? address = null,
        string? phone = null,
        string? email = null,
        string? website = null,
        bool isPublic = false)
    {
        var normalizedName = ValidateName(name);
        var normalizedSlug = ValidateSlug(slug);

        return new Institution(
            normalizedName,
            normalizedSlug,
            description,
            logoUrl,
            coverImageUrl,
            address,
            phone,
            email,
            website,
            isPublic);
    }

    public void UpdateProfile(
        string name,
        string? description,
        string? address,
        string? phone,
        string? email,
        string? website)
    {
        Name = ValidateName(name);
        Description = description;
        Address = address;
        Phone = phone;
        Email = email;
        Website = website;
        MarkAsUpdated();
    }

    public void UpdateSlug(string slug)
    {
        Slug = ValidateSlug(slug);
        MarkAsUpdated();
    }

    public void UpdateBranding(string? logoUrl, string? coverImageUrl)
    {
        LogoUrl = logoUrl;
        CoverImageUrl = coverImageUrl;
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
            throw new ValidationException("Institution name is required.");
        }

        return name.Trim();
    }

    private static string ValidateSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ValidationException("Institution slug is required.");
        }

        return slug.Trim().ToLowerInvariant();
    }
}
