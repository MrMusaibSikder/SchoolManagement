namespace SchoolERP.Application.Features.ExamSchedule.DTOs;

/// <summary>Read model returned to clients for a ExamSchedule record.</summary>
public class ExamScheduleDto
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string? ExamName { get; set; }
    public int ClassId { get; set; }
    public string? ClassName { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public DateTime ExamDate { get; set; }
    public int FullMarks { get; set; }
    public int PassMarks { get; set; }
}
