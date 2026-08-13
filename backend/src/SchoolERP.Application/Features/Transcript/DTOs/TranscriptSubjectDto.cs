namespace SchoolERP.Application.Features.Transcript.DTOs;

/// <summary>Subject-wise result row within a <see cref="TranscriptExamEntryDto"/> or a year summary.</summary>
public class TranscriptSubjectDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public decimal MarksObtained { get; set; }
    public string Grade { get; set; } = string.Empty;
    public decimal GPA { get; set; }
    public bool IsPassed { get; set; }

    /// <summary>Whether this subject was optional for the student's class (e.g. Higher Math, Agriculture, ICT Practical).</summary>
    public bool IsOptional { get; set; }
}
