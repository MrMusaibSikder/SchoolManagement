using CourseHub.Domain.Common;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Domain.Entities;

/// <summary>
/// RefreshToken represents an issued, revocable refresh token session for a
/// User. The Domain never sees the raw token value — only an opaque hash
/// computed by Infrastructure (see ITokenHasher in Application). Supports
/// rotation: when a token is used, it is revoked and linked to the token
/// that replaced it via ReplacedByTokenId.
/// </summary>
public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }

    /// <summary>
    /// Hash of the raw refresh token. The raw value is never persisted.
    /// </summary>
    public string TokenHash { get; private set; } = null!;

    public DateTime ExpiresAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    /// <summary>
    /// Set when this token was rotated out in favor of a newer one,
    /// enabling reuse detection (a revoked token used again is suspicious).
    /// </summary>
    public Guid? ReplacedByTokenId { get; private set; }

    public string? CreatedByIp { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsRevoked => RevokedAt.HasValue;

    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken()
    {
    }

    private RefreshToken(Guid userId, string tokenHash, DateTime expiresAt, string? createdByIp)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        CreatedByIp = createdByIp;
    }

    public static RefreshToken Create(Guid userId, string tokenHash, DateTime expiresAt, string? createdByIp = null)
    {
        if (userId == Guid.Empty)
        {
            throw new ValidationException("UserId is required.");
        }

        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new ValidationException("TokenHash is required.");
        }

        return new RefreshToken(userId, tokenHash, expiresAt.ToUniversalTime(), createdByIp);
    }

    /// <summary>
    /// Revokes this token. Pass the id of the token that replaced it when
    /// revoking as part of rotation, or leave null for a plain revoke
    /// (e.g. explicit logout).
    /// </summary>
    public void Revoke(Guid? replacedByTokenId = null)
    {
        if (IsRevoked)
        {
            return;
        }

        RevokedAt = DateTime.UtcNow;
        ReplacedByTokenId = replacedByTokenId;
        MarkAsUpdated();
    }
}
