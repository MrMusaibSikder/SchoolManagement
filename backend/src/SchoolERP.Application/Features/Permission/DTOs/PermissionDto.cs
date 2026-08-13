namespace SchoolERP.Application.Features.Permission.DTOs;

/// <summary>Read model returned to clients for a Permission record.</summary>
public class PermissionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
