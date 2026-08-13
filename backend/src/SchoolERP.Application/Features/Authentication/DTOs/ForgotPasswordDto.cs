namespace SchoolERP.Application.Features.Authentication.DTOs;

/// <summary>Input model for requesting a password reset token/email.</summary>
public class ForgotPasswordDto
{
    public string Email { get; set; } = string.Empty;
}
