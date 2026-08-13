using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Result.DTOs;

/// <summary>
/// Input model for entering a single student's mark for one exam schedule
/// (subject). The requesting teacher must be assigned to that subject via
/// the existing SubjectTeacher mapping.
/// </summary>
public class CreateResultDto
{
    public int StudentId { get; set; }
    public int ExamScheduleId { get; set; }

    /// <summary>Id of the teacher entering the mark. Must be assigned to the schedule's subject.</summary>
    public int TeacherId { get; set; }

    public decimal MarksObtained { get; set; }
    public decimal GraceMarks { get; set; }
    public MarkAttendanceStatus AttendanceStatus { get; set; } = MarkAttendanceStatus.Present;
    public string? Remarks { get; set; }
}
