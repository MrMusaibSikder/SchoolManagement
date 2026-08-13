namespace SchoolERP.Application.Features.Notice.DTOs;

/// <summary>Aggregate notice-board statistics for the admin dashboard.</summary>
public class NoticeDashboardSummaryDto
{
    public int TotalNotices { get; set; }
    public int DraftNotices { get; set; }
    public int PublishedNotices { get; set; }
    public int ArchivedNotices { get; set; }
    public int ActiveNotices { get; set; }
    public int UpcomingNotices { get; set; }
    public int ExpiredNotices { get; set; }
    public int HighPriorityNotices { get; set; }
}
