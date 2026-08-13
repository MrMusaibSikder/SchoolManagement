namespace SchoolERP.Application.Features.Transcript.DTOs;

/// <summary>Attendance summary for a student over the transcript's covered period, reused read-only from the existing Attendance module.</summary>
public class TranscriptAttendanceSummaryDto
{
    public int TotalDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
    public int LeaveDays { get; set; }

    /// <summary>Percentage of TotalDays marked Present.</summary>
    public decimal AttendancePercentage { get; set; }
}
