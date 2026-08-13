using SchoolERP.Application.Features.ExamSchedule.DTOs;

namespace SchoolERP.Application.Features.Exam.DTOs;

/// <summary>Subject-wise exam routine for a single class within a single exam.</summary>
public class ClassRoutineDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public IReadOnlyList<ExamScheduleDto> Schedules { get; set; } = Array.Empty<ExamScheduleDto>();
}
