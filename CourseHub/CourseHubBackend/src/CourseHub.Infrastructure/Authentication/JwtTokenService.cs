using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Common.Options;
using CourseHub.Application.Common.Security;
using CourseHub.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CourseHub.Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AccessTokenResult GenerateAccessToken(User user, IReadOnlyList<string> roles, IReadOnlyList<string> permissions)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // ClaimTypes.Role is what ASP.NET Core's role-checking APIs
        // (User.IsInRole, [Authorize(Roles=...)]) read by convention —
        // using it now means Phase 9's permission/role checks can build on
        // top of these tokens without reissuing them differently.
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Phase 9: effective permissions resolved from the user's roles,
        // baked in as a custom claim so PermissionAuthorizationHandler can
        // authorize requests without a DB round-trip per request.
        claims.AddRange(permissions.Select(permission => new Claim(PermissionClaimTypes.Permission, permission)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenResult(tokenString, expiresAtUtc);
    }
}
