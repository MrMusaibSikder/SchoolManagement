using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>
/// Represents a long-lived refresh token issued to a <see cref="User"/> alongside a
/// short-lived JWT access token. Used to obtain new access tokens without requiring
/// the user to re-authenticate, and to support token revocation on logout.
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>Id of the user this refresh token belongs to.</summary>
    public int UserId { get; set; }

    /// <summary>Navigation property to the owning user.</summary>
    public User User { get; set; } = null!;

    /// <summary>The opaque, cryptographically random token value.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>UTC timestamp when this token expires.</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>UTC timestamp when this token was revoked, if any.</summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// The value of the new token that replaced this one when it was rotated
    /// during a refresh operation. Null if this token was never rotated.
    /// </summary>
    public string? ReplacedByToken { get; set; }

    /// <summary>IP address of the client that requested this token, for auditing.</summary>
    public string? CreatedByIp { get; set; }

    /// <summary>True when the token has not expired and has not been revoked.</summary>
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;
}
