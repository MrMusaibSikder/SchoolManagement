namespace SchoolERP.Application.Features.Permission.DTOs;

/// <summary>Input model for updating an existing Permission record.</summary>
public class UpdatePermissionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
