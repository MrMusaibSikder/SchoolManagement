namespace SchoolERP.Application.Features.Exam.DTOs;

/// <summary>A single calendar entry (one exam schedule) for rendering an exam calendar/timetable view.</summary>
public class ExamCalendarDto
{
    public int ScheduleId { get; set; }
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public int FullMarks { get; set; }
    public int PassMarks { get; set; }
}
