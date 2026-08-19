using CourseHub.Domain.Common;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Domain.Entities;

/// <summary>
/// Student is the business/domain profile of a learner. It references the
/// User authentication identity via UserId but holds its own
/// domain-specific data (guardian info, contact, enrollment identity).
/// </summary>
public class Student : BaseEntity
{
    public Guid UserId { get; private set; }

    public string StudentId { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public string? ProfileImageUrl { get; private set; }

    public DateTime? DateOfBirth { get; private set; }

    public string? Phone { get; private set; }

    public string? Email { get; private set; }

    public string? Address { get; private set; }

    public string? GuardianName { get; private set; }

    public string? GuardianPhone { get; private set; }

    public bool IsActive { get; private set; }

    public bool IsProfilePublic { get; private set; }

    private Student()
    {
    }

    private Student(Guid userId, string studentId, string firstName, string lastName)
    {
        UserId = userId;
        StudentId = studentId;
        FirstName = firstName;
        LastName = lastName;
        IsActive = true;
    }

    public static Student Create(Guid userId, string studentId, string firstName, string lastName)
    {
        if (userId == Guid.Empty)
        {
            throw new ValidationException("UserId is required.");
        }

        var validatedStudentId = ValidateRequired(studentId, "StudentId");
        var validatedFirstName = ValidateRequired(firstName, "FirstName");
        var validatedLastName = ValidateRequired(lastName, "LastName");

        return new Student(userId, validatedStudentId, validatedFirstName, validatedLastName);
    }

    public void UpdateProfile(string firstName, string lastName, DateTime? dateOfBirth)
    {
        FirstName = ValidateRequired(firstName, "FirstName");
        LastName = ValidateRequired(lastName, "LastName");
        DateOfBirth = dateOfBirth;
        MarkAsUpdated();
    }

    public void UpdateContact(string? phone, string? email, string? address)
    {
        Phone = phone;
        Email = email;
        Address = address;
        MarkAsUpdated();
    }

    public void UpdateGuardian(string? guardianName, string? guardianPhone)
    {
        GuardianName = guardianName;
        GuardianPhone = guardianPhone;
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
