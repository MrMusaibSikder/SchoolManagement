using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>
/// Represents a single-use token issued to a <see cref="User"/> as part of the
/// Forgot Password / Reset Password flow.
/// </summary>
public class PasswordResetToken : BaseEntity
{
    /// <summary>Id of the user this reset token belongs to.</summary>
    public int UserId { get; set; }

    /// <summary>Navigation property to the owning user.</summary>
    public User User { get; set; } = null!;

    /// <summary>The opaque, cryptographically random token value sent to the user.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>UTC timestamp when this token expires.</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>Whether the token has already been consumed to reset a password.</summary>
    public bool IsUsed { get; set; }

    /// <summary>UTC timestamp when the token was used, if any.</summary>
    public DateTime? UsedAt { get; set; }

    /// <summary>True when the token has not expired and has not been used yet.</summary>
    public bool IsActive => !IsUsed && DateTime.UtcNow < ExpiresAt;
}
