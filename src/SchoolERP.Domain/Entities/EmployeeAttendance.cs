using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents a single day's attendance record for an employee.</summary>
public class EmployeeAttendance : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public DateTime AttendanceDate { get; set; } = DateTime.Today;
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }

    public AttendanceStatus Status { get; set; }
}
