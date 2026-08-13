using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.StudentAttendance.DTOs;

/// <summary>Input model for creating a new StudentAttendance record.</summary>
public class CreateStudentAttendanceDto
{
    public int StudentId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Remarks { get; set; }
}
