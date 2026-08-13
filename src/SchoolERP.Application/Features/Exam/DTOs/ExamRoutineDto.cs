using SchoolERP.Application.Features.ExamSchedule.DTOs;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Exam.DTOs;

/// <summary>Full subject-wise routine (schedule list) for a single exam, optionally scoped to one class.</summary>
public class ExamRoutineDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string ExamTypeName { get; set; } = string.Empty;
    public string AcademicYearName { get; set; } = string.Empty;
    public ExamStatus Status { get; set; }
    public IReadOnlyList<ExamScheduleDto> Schedules { get; set; } = Array.Empty<ExamScheduleDto>();
}
