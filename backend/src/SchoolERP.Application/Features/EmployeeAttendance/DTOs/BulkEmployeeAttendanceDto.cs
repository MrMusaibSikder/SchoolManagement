using System;
using System.Collections.Generic;

namespace SchoolERP.Application.Features.EmployeeAttendance.DTOs
{
    /// <summary>
    /// Input model for taking staff attendance for many employees at once, for a
    /// single date. Mirrors <c>BulkStudentAttendanceDto</c>, but since employees
    /// are not grouped by class/section, the payload is simply a date plus a list
    /// of per-employee attendance entries.
    /// </summary>
    public class BulkEmployeeAttendanceDto
    {
        /// <summary>The date attendance is being recorded for.</summary>
        public DateTime AttendanceDate { get; set; }

        /// <summary>The attendance entries to create or update.</summary>
        public List<EmployeeAttendanceItemDto> Attendance { get; set; } = new();
    }
}
