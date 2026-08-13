namespace SchoolERP.Application.Features.Role.DTOs;

/// <summary>Input model for creating a new Role record.</summary>
public class CreateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
