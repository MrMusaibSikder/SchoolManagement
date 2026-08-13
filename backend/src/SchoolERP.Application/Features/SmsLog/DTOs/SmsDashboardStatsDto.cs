namespace SchoolERP.Application.Features.SmsLog.DTOs;

/// <summary>Aggregate SMS statistics for the admin dashboard.</summary>
public class SmsDashboardStatsDto
{
    public int TotalSms { get; set; }
    public int TodaySms { get; set; }
    public int WeeklySms { get; set; }
    public int MonthlySms { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public int PendingCount { get; set; }

    /// <summary>Percentage (0-100) of Delivered/Sent messages out of all non-pending messages.</summary>
    public double SuccessRate { get; set; }
}
