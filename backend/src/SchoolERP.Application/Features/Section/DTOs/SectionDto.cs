namespace SchoolERP.Application.Features.Section.DTOs;

/// <summary>Read model returned to clients for a Section record.</summary>
public class SectionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ClassId { get; set; }
}
