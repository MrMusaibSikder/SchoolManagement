namespace SchoolERP.Application.Features.SchoolClass.DTOs;

/// <summary>Read model returned to clients for a SchoolClass record.</summary>
public class SchoolClassDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
