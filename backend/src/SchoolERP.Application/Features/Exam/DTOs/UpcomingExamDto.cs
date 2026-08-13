namespace SchoolERP.Application.Features.Exam.DTOs;

/// <summary>A published exam whose next schedule date is still in the future, for "upcoming exams" widgets.</summary>
public class UpcomingExamDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string ExamTypeName { get; set; } = string.Empty;

    /// <summary>The earliest not-yet-occurred schedule date for this exam.</summary>
    public DateTime NextExamDate { get; set; }

    /// <summary>Number of whole days remaining until <see cref="NextExamDate"/>.</summary>
    public int DaysRemaining { get; set; }

    public int TotalSchedules { get; set; }
}
