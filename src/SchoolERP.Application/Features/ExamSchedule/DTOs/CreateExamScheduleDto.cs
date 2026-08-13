namespace SchoolERP.Application.Features.ExamSchedule.DTOs;

/// <summary>Input model for creating a new ExamSchedule record.</summary>
public class CreateExamScheduleDto
{
    public int ExamId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public DateTime ExamDate { get; set; }
    public int FullMarks { get; set; }
    public int PassMarks { get; set; }
}
