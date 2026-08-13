using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Result.DTOs;

/// <summary>Read model returned to clients for a Result (mark entry) record.</summary>
public class ResultDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? RollNo { get; set; }
    public int ExamScheduleId { get; set; }
    public string? ExamName { get; set; }
    public string? SubjectName { get; set; }
    public int FullMarks { get; set; }
    public int PassMarks { get; set; }

    public decimal MarksObtained { get; set; }
    public decimal GraceMarks { get; set; }
    public string? Grade { get; set; }
    public decimal? GPA { get; set; }
    public bool IsPassed { get; set; }
    public decimal? Percentage { get; set; }

    public MarkAttendanceStatus AttendanceStatus { get; set; }
    public MarkEntryStatus EntryStatus { get; set; }
    public string? Remarks { get; set; }

    public bool IsLocked { get; set; }
    public DateTime? LockedAt { get; set; }

    public int? EnteredByTeacherId { get; set; }
    public string? EnteredByTeacherName { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
