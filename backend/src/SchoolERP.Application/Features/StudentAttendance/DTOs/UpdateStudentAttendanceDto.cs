using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.StudentAttendance.DTOs;

/// <summary>Input model for updating an existing StudentAttendance record.</summary>
public class UpdateStudentAttendanceDto
{
    
    public AttendanceStatus Status { get; set; }
    public string? Remarks { get; set; }
}
