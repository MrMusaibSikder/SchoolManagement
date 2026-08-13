namespace SchoolERP.Application.Features.Subject.DTOs;

/// <summary>Read model returned to clients for a Subject record.</summary>
public class SubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int FullMarks { get; set; }
    public int PassMarks { get; set; }
}
