using SchoolERP.Application.Features.ExamSchedule.DTOs;

namespace SchoolERP.Application.Features.Exam.DTOs;

/// <summary>Invigilation/subject routine for a single teacher, across the subjects they teach within an exam.</summary>
public class TeacherRoutineDto
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int? ExamId { get; set; }
    public string? ExamName { get; set; }
    public IReadOnlyList<ExamScheduleDto> Schedules { get; set; } = Array.Empty<ExamScheduleDto>();
}
