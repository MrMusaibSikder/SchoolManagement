namespace SchoolERP.Application.Features.Authentication.DTOs;

public class LoginResponseDto
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = new();

    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    /// <summary>Opaque refresh token that can be exchanged for a new access token via /api/Auth/refresh-token.</summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>UTC expiry of <see cref="RefreshToken"/>.</summary>
    public DateTime RefreshTokenExpiresAt { get; set; }
}