using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Result.DTOs;

/// <summary>
/// Input model for updating an existing mark entry. Student/ExamSchedule
/// cannot be changed via update — delete and re-enter instead.
/// </summary>
public class UpdateResultDto
{
    public int Id { get; set; }

    /// <summary>Id of the teacher performing the update. Must be assigned to the schedule's subject.</summary>
    public int TeacherId { get; set; }

    public decimal MarksObtained { get; set; }
    public decimal GraceMarks { get; set; }
    public MarkAttendanceStatus AttendanceStatus { get; set; }
    public string? Remarks { get; set; }
}
