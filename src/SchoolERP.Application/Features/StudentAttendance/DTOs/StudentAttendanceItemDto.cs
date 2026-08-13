using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.StudentAttendance.DTOs
{
    public class StudentAttendanceItemDto
    {
        public int StudentId { get; set; }

        public AttendanceStatus Status { get; set; }

        public string? Remarks { get; set; }
    }
}
