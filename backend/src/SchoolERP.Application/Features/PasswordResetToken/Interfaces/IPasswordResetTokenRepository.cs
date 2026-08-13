using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Features.PasswordResetToken.Interfaces;

/// <summary>
/// Repository contract for <see cref="SchoolERP.Domain.Entities.PasswordResetToken"/> entities.
/// Extends the generic repository with reset-token-specific data access members.
/// Contains database operations only.
/// </summary>
public interface IPasswordResetTokenRepository : IGenericRepository<SchoolERP.Domain.Entities.PasswordResetToken>
{
    /// <summary>Finds a password reset token by its opaque token value.</summary>
    Task<SchoolERP.Domain.Entities.PasswordResetToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>Gets every currently active (non-expired, unused) reset token for a user.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.PasswordResetToken>> GetActiveByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
