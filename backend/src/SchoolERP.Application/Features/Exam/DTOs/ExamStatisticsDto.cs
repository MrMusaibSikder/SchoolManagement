using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Exam.DTOs;

/// <summary>Aggregate scheduling statistics for a single exam (no marks/results — that belongs to the future Result module).</summary>
public class ExamStatisticsDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public ExamStatus Status { get; set; }
    public int TotalSchedules { get; set; }
    public int TotalSubjects { get; set; }
    public int TotalClasses { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    /// <summary>Whole days spanned from <see cref="StartDate"/> to <see cref="EndDate"/> (0 if only a single day).</summary>
    public int DurationInDays { get; set; }
}
