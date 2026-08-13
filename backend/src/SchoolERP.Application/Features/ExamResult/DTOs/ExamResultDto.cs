namespace SchoolERP.Application.Features.ExamResult.DTOs;

/// <summary>Aggregate, per-student result for a single exam.</summary>
public class ExamResultDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string RollNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;

    public decimal TotalMarks { get; set; }
    public decimal TotalFullMarks { get; set; }
    public decimal Percentage { get; set; }
    public decimal GPA { get; set; }
    public string Grade { get; set; } = string.Empty;
    public bool IsPassed { get; set; }

    public int? MeritPosition { get; set; }
    public int? ClassPosition { get; set; }
    public int? SectionPosition { get; set; }

    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }

    /// <summary>Optional subject/class teacher's remark for this exam.</summary>
    public string? TeacherRemarks { get; set; }

    /// <summary>Optional guardian-facing remark for this exam.</summary>
    public string? GuardianRemarks { get; set; }
}
