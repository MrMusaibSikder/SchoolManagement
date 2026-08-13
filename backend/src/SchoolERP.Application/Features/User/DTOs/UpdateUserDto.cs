namespace SchoolERP.Application.Features.User.DTOs;

/// <summary>Input model for updating an existing User record.</summary>
public class UpdateUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Password { get; set; } = string.Empty;
}
