namespace SchoolERP.Application.Features.Authentication.DTOs;

/// <summary>Input model for completing a password reset using a previously issued token.</summary>
public class ResetPasswordDto
{
    /// <summary>The reset token received via the Forgot Password flow.</summary>
    public string Token { get; set; } = string.Empty;

    public string NewPassword { get; set; } = string.Empty;

    public string ConfirmNewPassword { get; set; } = string.Empty;
}
