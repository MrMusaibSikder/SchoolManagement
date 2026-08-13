namespace SchoolERP.Application.Features.SchoolClass.DTOs;

/// <summary>Input model for updating an existing SchoolClass record.</summary>
public class UpdateSchoolClassDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
