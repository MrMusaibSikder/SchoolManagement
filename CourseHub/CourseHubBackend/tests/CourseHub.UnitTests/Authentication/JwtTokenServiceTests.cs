using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CourseHub.Application.Common.Options;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Authentication;
using Microsoft.Extensions.Options;

namespace CourseHub.UnitTests.Authentication;

public class JwtTokenServiceTests
{
    private static JwtTokenService CreateService(JwtOptions? options = null)
    {
        options ??= new JwtOptions
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            SecretKey = "this-is-a-test-only-secret-key-32-bytes-min",
            AccessTokenExpirationMinutes = 15,
        };

        return new JwtTokenService(Options.Create(options));
    }

    private static User CreateUser()
    {
        return User.Create("user@example.com", "hashed-password", "Jane", "Doe");
    }

    [Fact]
    public void GenerateAccessToken_ReturnsNonEmptyToken()
    {
        var sut = CreateService();
        var user = CreateUser();

        var result = sut.GenerateAccessToken(user, new[] { "Student" }, Array.Empty<string>());

        Assert.False(string.IsNullOrWhiteSpace(result.Token));
    }

    [Fact]
    public void GenerateAccessToken_IncludesExpectedClaims()
    {
        var sut = CreateService();
        var user = CreateUser();

        var result = sut.GenerateAccessToken(user, new[] { "Teacher", "CourseCoordinator" }, Array.Empty<string>());

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Token);

        Assert.Equal(user.Id.ToString(), token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(user.Email, token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Jti);

        var roleClaims = token.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        Assert.Contains("Teacher", roleClaims);
        Assert.Contains("CourseCoordinator", roleClaims);
    }

    [Fact]
    public void GenerateAccessToken_NoRoles_ProducesNoRoleClaims()
    {
        var sut = CreateService();
        var user = CreateUser();

        var result = sut.GenerateAccessToken(user, Array.Empty<string>(), Array.Empty<string>());

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Token);

        Assert.DoesNotContain(token.Claims, c => c.Type == ClaimTypes.Role);
    }

    [Fact]
    public void GenerateAccessToken_IncludesPermissionClaims()
    {
        var sut = CreateService();
        var user = CreateUser();

        var result = sut.GenerateAccessToken(user, new[] { "Admin" }, new[] { "roles.manage", "roles.view" });

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Token);

        var permissionClaims = token.Claims
            .Where(c => c.Type == CourseHub.Application.Common.Security.PermissionClaimTypes.Permission)
            .Select(c => c.Value)
            .ToList();

        Assert.Contains("roles.manage", permissionClaims);
        Assert.Contains("roles.view", permissionClaims);
    }

    [Fact]
    public void GenerateAccessToken_SetsExpirationFromOptions()
    {
        var options = new JwtOptions
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            SecretKey = "this-is-a-test-only-secret-key-32-bytes-min",
            AccessTokenExpirationMinutes = 30,
        };
        var sut = CreateService(options);
        var user = CreateUser();

        var before = DateTime.UtcNow.AddMinutes(30);
        var result = sut.GenerateAccessToken(user, new[] { "Student" }, Array.Empty<string>());
        var after = DateTime.UtcNow.AddMinutes(30);

        Assert.InRange(result.ExpiresAtUtc, before.AddSeconds(-5), after.AddSeconds(5));
    }
}
