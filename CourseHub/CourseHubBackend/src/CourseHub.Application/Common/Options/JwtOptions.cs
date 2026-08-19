namespace CourseHub.Application.Common.Options;

/// <summary>
/// Strongly typed JWT configuration, bound from "Authentication:Jwt".
/// SecretKey must come from User Secrets (dev) or environment
/// variables/secret manager (production) — never from appsettings.json.
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Authentication:Jwt";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public int AccessTokenExpirationMinutes { get; set; } = 15;

    public int RefreshTokenExpirationDays { get; set; } = 7;
}
