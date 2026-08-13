using SchoolERP.Application.Features.ExamSchedule.DTOs;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Exam.DTOs;

/// <summary>Full detail view of a single exam: its own fields plus every schedule under it.</summary>
public class ExamDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ExamTypeId { get; set; }
    public string ExamTypeName { get; set; } = string.Empty;
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public ExamStatus Status { get; set; }
    public int TotalSchedules { get; set; }
    public IReadOnlyList<ExamScheduleDto> Schedules { get; set; } = Array.Empty<ExamScheduleDto>();
}
