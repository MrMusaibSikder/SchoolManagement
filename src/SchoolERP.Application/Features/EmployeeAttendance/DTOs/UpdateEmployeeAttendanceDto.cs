using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.EmployeeAttendance.DTOs;

/// <summary>Input model for updating an existing EmployeeAttendance record.</summary>
public class UpdateEmployeeAttendanceDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public AttendanceStatus Status { get; set; }
}
