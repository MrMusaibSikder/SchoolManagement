namespace SchoolERP.Application.Features.ExamResult.DTOs;

/// <summary>Summary statistics for a single exam's result-processing progress and outcomes.</summary>
public class ExamResultDashboardDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;

    public int TotalStudents { get; set; }
    public int AppearedStudents { get; set; }
    public int AbsentStudents { get; set; }

    public int TotalScheduleCount { get; set; }
    public int FullySubmittedScheduleCount { get; set; }

    /// <summary>Percentage of exam schedules whose marks are fully Submitted.</summary>
    public decimal CompletionPercentage { get; set; }

    public bool IsResultPublished { get; set; }
    public int PublishedResultCount { get; set; }
    public int PendingResultCount { get; set; }

    public IReadOnlyList<SubjectStatisticsDto> SubjectStatistics { get; set; } = Array.Empty<SubjectStatisticsDto>();
}
