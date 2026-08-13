using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Transcript.DTOs;

/// <summary>A single academic year's weighted final outcome within a student's transcript.</summary>
public class TranscriptYearSummaryDto
{
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public decimal FinalGPA { get; set; }
    public string FinalGrade { get; set; } = string.Empty;
    public bool IsPassed { get; set; }
    public PromotionStatus PromotionStatus { get; set; }
    public int? ClassPosition { get; set; }
    public int? SectionPosition { get; set; }
    public int? MeritPosition { get; set; }
    public DateTime? PublishedAt { get; set; }

    /// <summary>Subject-wise weighted breakdown for this academic year.</summary>
    public IReadOnlyList<TranscriptSubjectDto> Subjects { get; set; } = Array.Empty<TranscriptSubjectDto>();

    /// <summary>Optional class/subject teacher's remark for the year.</summary>
    public string? TeacherRemarks { get; set; }

    /// <summary>Optional principal/head-teacher remark for the year.</summary>
    public string? PrincipalRemarks { get; set; }
}
