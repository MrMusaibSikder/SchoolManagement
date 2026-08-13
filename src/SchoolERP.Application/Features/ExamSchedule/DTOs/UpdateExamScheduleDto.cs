namespace SchoolERP.Application.Features.ExamSchedule.DTOs;

/// <summary>Input model for updating an existing ExamSchedule record.</summary>
public class UpdateExamScheduleDto
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public DateTime ExamDate { get; set; }
    public int FullMarks { get; set; }
    public int PassMarks { get; set; }
}
