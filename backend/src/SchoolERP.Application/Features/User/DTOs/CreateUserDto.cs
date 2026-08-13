namespace SchoolERP.Application.Features.User.DTOs;

/// <summary>Input model for creating a new User record.</summary>
public class CreateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Password { get; set; } = string.Empty;
}
