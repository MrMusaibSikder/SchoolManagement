using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.StudentAttendance.DTOs
{
    public class BulkStudentAttendanceDto
    {
        public int ClassId { get; set; }

        public int SectionId { get; set; }

        public DateTime AttendanceDate { get; set; }

        public List<StudentAttendanceItemDto> Attendance { get; set; } = new();
    }
}
