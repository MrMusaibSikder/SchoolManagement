namespace SchoolERP.Application.Features.Subject.DTOs;

/// <summary>Input model for creating a new Subject record.</summary>
public class CreateSubjectDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int FullMarks { get; set; }
    public int PassMarks { get; set; }
}
