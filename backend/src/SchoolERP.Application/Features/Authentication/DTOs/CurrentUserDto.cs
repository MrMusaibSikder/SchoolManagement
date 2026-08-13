namespace SchoolERP.Application.Features.Authentication.DTOs;

/// <summary>Read model describing the currently authenticated user's profile.</summary>
public class CurrentUserDto
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public List<string> Roles { get; set; } = new();

    public List<string> Permissions { get; set; } = new();
}
