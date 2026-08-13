using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user, IList<string> roles);

    /// <summary>
    /// changed 
    /// Generates a cryptographically random, URL-safe refresh token value.
    /// The token itself carries no claims; it is an opaque lookup key persisted
    /// via <see cref="Features.Authentication.Interfaces.IAuthService"/> against a
    /// <see cref="Domain.Entities.RefreshToken"/> row.
    /// </summary>
    string GenerateRefreshToken();
}