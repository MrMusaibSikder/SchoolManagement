using SchoolERP.Application.Features.Authentication.DTOs;

namespace SchoolERP.Application.Features.Authentication.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);

    /// <summary>
    /// Exchanges a valid, active refresh token for a new access token and a new
    /// (rotated) refresh token. Returns null if the supplied token is missing,
    /// expired, revoked, or unknown.
    /// </summary>
    Task<RefreshTokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);

    /// <summary>
    /// Revokes the given refresh token (or, if none is supplied, every active
    /// refresh token belonging to <paramref name="userId"/>), effectively logging
    /// the user out of that device or all devices.
    /// </summary>
    Task LogoutAsync(int userId, LogoutRequestDto request);

    /// <summary>Changes the password for an authenticated user after verifying their current password.</summary>
    Task ChangePasswordAsync(int userId, ChangePasswordDto request);

    /// <summary>
    /// Issues a password reset token for the account matching the given email, if
    /// one exists, and emails it to the user. Always completes successfully
    /// (no user enumeration) regardless of whether the email was found.
    /// </summary>
    Task ForgotPasswordAsync(ForgotPasswordDto request);

    /// <summary>Completes a password reset using a token issued by <see cref="ForgotPasswordAsync"/>.</summary>
    Task ResetPasswordAsync(ResetPasswordDto request);
}