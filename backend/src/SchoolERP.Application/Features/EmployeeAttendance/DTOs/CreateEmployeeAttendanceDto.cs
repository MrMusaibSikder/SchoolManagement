using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.EmployeeAttendance.DTOs;

/// <summary>Input model for creating a new EmployeeAttendance record.</summary>
public class CreateEmployeeAttendanceDto
{
    public int EmployeeId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public AttendanceStatus Status { get; set; }
}
