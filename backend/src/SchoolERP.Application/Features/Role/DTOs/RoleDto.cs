namespace SchoolERP.Application.Features.Role.DTOs;

/// <summary>Read model returned to clients for a Role record.</summary>
public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
