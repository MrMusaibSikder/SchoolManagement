namespace SchoolERP.Application.Features.Authentication.DTOs;

/// <summary>Input model for exchanging a refresh token for a new access token.</summary>
public class RefreshTokenRequestDto
{
    /// <summary>The refresh token previously issued at login.</summary>
    public string RefreshToken { get; set; } = string.Empty;
}
