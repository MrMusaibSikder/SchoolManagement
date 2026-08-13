namespace SchoolERP.Application.Features.SmsLog.DTOs;

/// <summary>Single month's SMS delivery totals, used to build monthly report charts.</summary>
public class SmsMonthlyReportDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Success { get; set; }
    public int Failed { get; set; }
    public int Pending { get; set; }
}
