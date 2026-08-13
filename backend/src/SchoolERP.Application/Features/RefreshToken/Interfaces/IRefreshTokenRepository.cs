using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Features.RefreshToken.Interfaces;

/// <summary>
/// Repository contract for <see cref="SchoolERP.Domain.Entities.RefreshToken"/> entities.
/// Extends the generic repository with refresh-token-specific data access members.
/// Contains database operations only.
/// </summary>
public interface IRefreshTokenRepository : IGenericRepository<SchoolERP.Domain.Entities.RefreshToken>
{
    /// <summary>Finds a refresh token by its opaque token value.</summary>
    Task<SchoolERP.Domain.Entities.RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>Gets every currently active (non-expired, non-revoked) token for a user.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.RefreshToken>> GetActiveByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
