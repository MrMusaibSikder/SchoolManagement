using CourseHub.Domain.Common;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Domain.Entities;

/// <summary>
/// PasswordResetToken represents a single-use, time-limited password reset
/// request for a User. Like RefreshToken, only an opaque hash of the raw
/// token is stored — the raw value is emailed to the user and never
/// persisted or logged.
/// </summary>
public class PasswordResetToken : BaseEntity
{
    public Guid UserId { get; private set; }

    public string TokenHash { get; private set; } = null!;

    public DateTime ExpiresAt { get; private set; }

    public DateTime? UsedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsUsed => UsedAt.HasValue;

    public bool IsValid => !IsUsed && !IsExpired;

    private PasswordResetToken()
    {
    }

    private PasswordResetToken(Guid userId, string tokenHash, DateTime expiresAt)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
    }

    public static PasswordResetToken Create(Guid userId, string tokenHash, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
        {
            throw new ValidationException("UserId is required.");
        }

        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new ValidationException("TokenHash is required.");
        }

        return new PasswordResetToken(userId, tokenHash, expiresAt.ToUniversalTime());
    }

    public void MarkAsUsed()
    {
        if (IsUsed)
        {
            return;
        }

        UsedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }
}
