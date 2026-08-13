using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.StudentAttendance.DTOs;

/// <summary>Read model returned to clients for a StudentAttendance record.</summary>
public class StudentAttendanceDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public DateTime AttendanceDate { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Remarks { get; set; }
}
