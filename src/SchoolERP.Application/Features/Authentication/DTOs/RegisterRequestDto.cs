namespace SchoolERP.Application.Features.Authentication.DTOs;

public class RegisterRequestDto
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    // Default Role
    public string RoleName { get; set; } = "Student";
}