namespace SchoolERP.Application.Features.Designation.DTOs;

/// <summary>Read model returned to clients for a Designation record.</summary>
public class DesignationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
