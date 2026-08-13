namespace SchoolERP.Application.Features.SmsLog.DTOs;

/// <summary>Single day's SMS delivery totals, used to build daily report charts.</summary>
public class SmsDailyReportDto
{
    public DateTime Date { get; set; }
    public int Total { get; set; }
    public int Success { get; set; }
    public int Failed { get; set; }
    public int Pending { get; set; }
}
