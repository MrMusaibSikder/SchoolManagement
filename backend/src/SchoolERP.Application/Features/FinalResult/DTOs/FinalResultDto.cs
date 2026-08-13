using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.FinalResult.DTOs;

/// <summary>Read model returned to clients for a student's weighted final result for an academic year.</summary>
public class FinalResultDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string RollNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;

    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public int ExamWeightSetupId { get; set; }

    public decimal FinalMarks { get; set; }
    public decimal FinalGPA { get; set; }
    public string FinalGrade { get; set; } = string.Empty;
    public bool IsPassed { get; set; }
    public PromotionStatus PromotionStatus { get; set; }

    public int? MeritPosition { get; set; }
    public int? ClassPosition { get; set; }
    public int? SectionPosition { get; set; }

    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }

    /// <summary>Optional class/subject teacher's remark for the year.</summary>
    public string? TeacherRemarks { get; set; }

    /// <summary>Optional principal/head-teacher remark for the year.</summary>
    public string? PrincipalRemarks { get; set; }

    public IReadOnlyList<FinalResultDetailDto> Details { get; set; } = Array.Empty<FinalResultDetailDto>();
}
