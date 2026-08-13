namespace SchoolERP.Application.Features.Exam.DTOs;

/// <summary>Aggregate exam-board statistics and highlights for the admin dashboard.</summary>
public class ExamDashboardDto
{
    public int TotalExams { get; set; }
    public int DraftExams { get; set; }
    public int PublishedExams { get; set; }
    public int CompletedExams { get; set; }
    public int CancelledExams { get; set; }
    public int UpcomingExamsCount { get; set; }

    public IReadOnlyList<UpcomingExamDto> UpcomingExams { get; set; } = Array.Empty<UpcomingExamDto>();
    public IReadOnlyList<ExamSummaryDto> RecentExams { get; set; } = Array.Empty<ExamSummaryDto>();
}
