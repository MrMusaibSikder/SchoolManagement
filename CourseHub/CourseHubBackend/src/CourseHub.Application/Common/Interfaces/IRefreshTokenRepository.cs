using CourseHub.Domain.Entities;

namespace CourseHub.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns every currently-active (not revoked, not expired) refresh
    /// token for a user — used for reuse detection and for revoking every
    /// session on password change.
    /// </summary>
    Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
