namespace SchoolERP.Application.Features.Section.DTOs;

/// <summary>Input model for creating a new Section record.</summary>
public class CreateSectionDto
{
    public string Name { get; set; } = string.Empty;
    public int ClassId { get; set; }
}
