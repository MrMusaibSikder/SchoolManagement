namespace SchoolERP.Application.Features.Authentication.DTOs;

/// <summary>Input model for an authenticated user changing their own password.</summary>
public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;

    public string NewPassword { get; set; } = string.Empty;

    public string ConfirmNewPassword { get; set; } = string.Empty;
}
