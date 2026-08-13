namespace SchoolERP.Application.Common.Models;

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int DurationInMinutes { get; set; }
    /// <summary>
    /// Modified BY Musaib sikder.
    /// How many days a refresh token remains valid for.
    /// Added for the Refresh Token feature.
    /// </summary>
    public int RefreshTokenExpiryDays { get; set; } = 7;
}