namespace SchoolERP.Application.Features.Role.DTOs;

/// <summary>Input model for updating an existing Role record.</summary>
public class UpdateRoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
