using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.EmployeeAttendance.DTOs;

/// <summary>Read model returned to clients for a EmployeeAttendance record.</summary>
public class EmployeeAttendanceDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public AttendanceStatus Status { get; set; }
}
