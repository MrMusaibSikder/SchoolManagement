using CourseHub.Domain.Common;
using CourseHub.Domain.Enums;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Domain.Entities;

/// <summary>
/// User is the authentication identity of CourseHub.
/// Every user belongs to exactly one Institution (tenant).
/// Roles are NOT modeled here — they are assigned dynamically via UserRole -> Role
/// in a later phase. This entity intentionally has no Role property.
/// </summary>
public class User : BaseEntity
{
    public Guid InstitutionId { get; private set; }

    public string Email { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public string? ProfileImageUrl { get; private set; }

    public UserStatus Status { get; private set; }

    public DateTime? LastLoginAt { get; private set; }

    // EF Core requires a parameterless constructor; kept private so it
    // cannot be used to construct an invalid instance outside the entity.
    private User()
    {
    }

    private User(
        Guid institutionId,
        string email,
        string passwordHash,
        string firstName,
        string lastName)
    {
        InstitutionId = institutionId;
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        Status = UserStatus.Active;
    }

    public static User Create(
        Guid institutionId,
        string email,
        string passwordHash,
        string firstName,
        string lastName)
    {
        if (institutionId == Guid.Empty)
        {
            throw new ValidationException("InstitutionId is required.");
        }

        var normalizedEmail = ValidateEmail(email);
        var validatedPasswordHash = ValidatePasswordHash(passwordHash);
        var validatedFirstName = ValidateName(firstName, nameof(firstName));
        var validatedLastName = ValidateName(lastName, nameof(lastName));

        return new User(
            institutionId,
            normalizedEmail,
            validatedPasswordHash,
            validatedFirstName,
            validatedLastName);
    }

    public void UpdateProfile(string firstName, string lastName, string? profileImageUrl)
    {
        FirstName = ValidateName(firstName, nameof(firstName));
        LastName = ValidateName(lastName, nameof(lastName));
        ProfileImageUrl = profileImageUrl;
        MarkAsUpdated();
    }

    public void ChangePasswordHash(string newPasswordHash)
    {
        PasswordHash = ValidatePasswordHash(newPasswordHash);
        MarkAsUpdated();
    }

    public void Activate()
    {
        Status = UserStatus.Active;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        MarkAsUpdated();
    }

    public void Suspend()
    {
        Status = UserStatus.Suspended;
        MarkAsUpdated();
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    private static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ValidationException("Email is required.");
        }

        return email.Trim().ToLowerInvariant();
    }

    private static string ValidatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ValidationException("PasswordHash is required.");
        }

        return passwordHash;
    }

    private static string ValidateName(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException($"{fieldName} is required.");
        }

        return value.Trim();
    }
}
