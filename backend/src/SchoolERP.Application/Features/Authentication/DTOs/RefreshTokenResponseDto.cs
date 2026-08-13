namespace SchoolERP.Application.Features.Authentication.DTOs;

/// <summary>Result of a successful refresh-token exchange: a new access/refresh token pair.</summary>
public class RefreshTokenResponseDto
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = new();

    /// <summary>The newly issued JWT access token.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>UTC expiry of the new access token.</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>The newly issued (rotated) refresh token. Replaces the one sent in the request.</summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>UTC expiry of the new refresh token.</summary>
    public DateTime RefreshTokenExpiresAt { get; set; }
}
