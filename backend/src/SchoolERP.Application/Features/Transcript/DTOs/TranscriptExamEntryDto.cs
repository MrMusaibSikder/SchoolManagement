namespace SchoolERP.Application.Features.Transcript.DTOs;

/// <summary>A single published exam's outcome within a student's transcript exam history.</summary>
public class TranscriptExamEntryDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string ExamTypeName { get; set; } = string.Empty;
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public decimal GPA { get; set; }
    public string Grade { get; set; } = string.Empty;
    public bool IsPassed { get; set; }
    public DateTime? PublishedAt { get; set; }

    /// <summary>Subject-wise breakdown for this exam.</summary>
    public IReadOnlyList<TranscriptSubjectDto> Subjects { get; set; } = Array.Empty<TranscriptSubjectDto>();
}
