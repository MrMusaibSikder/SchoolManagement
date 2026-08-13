namespace SchoolERP.Application.Features.Authentication.DTOs;

/// <summary>
/// Input model for logging out. The refresh token is revoked so it can no longer
/// be used to mint new access tokens. If omitted, every active refresh token
/// belonging to the current user is revoked ("logout from all devices").
/// </summary>
public class LogoutRequestDto
{
    public string? RefreshToken { get; set; }
}
