using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.EmployeeAttendance.DTOs
{
    /// <summary>
    /// A single employee's attendance entry within a <see cref="BulkEmployeeAttendanceDto"/> request.
    /// </summary>
    public class EmployeeAttendanceItemDto
    {
        /// <summary>Id of the employee this entry belongs to.</summary>
        public int EmployeeId { get; set; }

        /// <summary>Attendance status (Present, Absent, Leave, etc.) for the day.</summary>
        public AttendanceStatus Status { get; set; }

        /// <summary>Optional check-in time for the day.</summary>
        public DateTime? CheckIn { get; set; }

        /// <summary>Optional check-out time for the day.</summary>
        public DateTime? CheckOut { get; set; }
    }
}
