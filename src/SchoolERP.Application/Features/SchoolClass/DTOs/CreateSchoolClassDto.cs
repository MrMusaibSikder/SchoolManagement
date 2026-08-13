namespace SchoolERP.Application.Features.SchoolClass.DTOs;

/// <summary>Input model for creating a new SchoolClass record.</summary>
public class CreateSchoolClassDto
{
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
