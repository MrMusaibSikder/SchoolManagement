namespace SchoolERP.Application.Features.Subject.DTOs;

/// <summary>Input model for updating an existing Subject record.</summary>
public class UpdateSubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int FullMarks { get; set; }
    public int PassMarks { get; set; }
}
