namespace SchoolERP.Application.Features.User.DTOs;

/// <summary>Read model returned to clients for a User record.</summary>
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
