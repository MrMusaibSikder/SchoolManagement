namespace SchoolERP.Application.Features.Designation.DTOs;

/// <summary>Input model for updating an existing Designation record.</summary>
public class UpdateDesignationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
