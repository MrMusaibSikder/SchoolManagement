namespace SchoolERP.Application.Features.ClassSubject.DTOs;

/// <summary>Data transfer object representing a ClassSubject association.</summary>
public class ClassSubjectDto
{
    public int ClassId { get; set; }
    public int SubjectId { get; set; }

    /// <summary>Whether this subject is optional for this class (e.g. Higher Math, Agriculture, ICT Practical).</summary>
    public bool IsOptional { get; set; }
}
