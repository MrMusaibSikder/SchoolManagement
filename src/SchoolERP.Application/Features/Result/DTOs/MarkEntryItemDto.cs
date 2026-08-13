using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Result.DTOs;

/// <summary>A single student's mark entry within a <see cref="BulkMarkEntryDto"/> request.</summary>
public class MarkEntryItemDto
{
    public int StudentId { get; set; }
    public decimal MarksObtained { get; set; }
    public decimal GraceMarks { get; set; }
    public MarkAttendanceStatus AttendanceStatus { get; set; } = MarkAttendanceStatus.Present;
    public string? Remarks { get; set; }
}
