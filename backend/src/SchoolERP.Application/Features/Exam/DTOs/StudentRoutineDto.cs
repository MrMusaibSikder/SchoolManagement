using SchoolERP.Application.Features.ExamSchedule.DTOs;

namespace SchoolERP.Application.Features.Exam.DTOs;

/// <summary>Personal exam routine for a single student (their class's schedules within an exam).</summary>
public class StudentRoutineDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public IReadOnlyList<ExamScheduleDto> Schedules { get; set; } = Array.Empty<ExamScheduleDto>();
}
