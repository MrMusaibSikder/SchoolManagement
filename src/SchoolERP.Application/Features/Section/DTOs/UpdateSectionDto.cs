namespace SchoolERP.Application.Features.Section.DTOs;

/// <summary>Input model for updating an existing Section record.</summary>
public class UpdateSectionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ClassId { get; set; }
}
