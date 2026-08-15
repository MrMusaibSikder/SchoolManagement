using CourseHub.Domain.Common;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Domain.Entities;

/// <summary>
/// Teacher is the institution-owned business/domain profile of a person
/// who teaches. It references the User authentication identity via UserId
/// but holds its own domain-specific data (bio, specialization, contact).
/// </summary>
public class Teacher : BaseEntity
{
    public Guid InstitutionId { get; private set; }

    public Guid UserId { get; private set; }

    /// <summary>
    /// Business identifier for the teacher within the institution.
    /// </summary>
    public string EmployeeId { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public string? ProfileImageUrl { get; private set; }

    public string? Phone { get; private set; }

    public string? Email { get; private set; }

    public string? Bio { get; private set; }

    public bool IsActive { get; private set; }

    public bool IsProfilePublic { get; private set; }

    private Teacher()
    {
    }

    private Teacher(
        Guid institutionId,
        Guid userId,
        string employeeId,
        string firstName,
        string lastName)
    {
        InstitutionId = institutionId;
        UserId = userId;
        EmployeeId = employeeId;
        FirstName = firstName;
        LastName = lastName;
        IsActive = true;
    }

    public static Teacher Create(
        Guid institutionId,
        Guid userId,
        string employeeId,
        string firstName,
        string lastName)
    {
        if (institutionId == Guid.Empty)
        {
            throw new ValidationException("InstitutionId is required.");
        }

        if (userId == Guid.Empty)
        {
            throw new ValidationException("UserId is required.");
        }

        var validatedEmployeeId = ValidateRequired(employeeId, "EmployeeId");
        var validatedFirstName = ValidateRequired(firstName, "FirstName");
        var validatedLastName = ValidateRequired(lastName, "LastName");

        return new Teacher(institutionId, userId, validatedEmployeeId, validatedFirstName, validatedLastName);
    }

    public void UpdateProfile(string firstName, string lastName, string? bio)
    {
        FirstName = ValidateRequired(firstName, "FirstName");
        LastName = ValidateRequired(lastName, "LastName");
        Bio = bio;
        MarkAsUpdated();
    }

    public void UpdateContact(string? phone, string? email)
    {
        Phone = phone;
        Email = email;
        MarkAsUpdated();
    }

    public void UpdateProfileImage(string? profileImageUrl)
    {
        ProfileImageUrl = profileImageUrl;
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

    public void MakeProfilePublic()
    {
        IsProfilePublic = true;
        MarkAsUpdated();
    }

    public void MakeProfilePrivate()
    {
        IsProfilePublic = false;
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
