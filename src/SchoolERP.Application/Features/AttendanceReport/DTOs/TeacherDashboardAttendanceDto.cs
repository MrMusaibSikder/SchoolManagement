using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.AttendanceReport.DTOs
{
    public class TeacherDashboardAttendanceDto
    {
        public int ClassId { get; set; }

        public int SectionId { get; set; }

        public int TotalStudents { get; set; }

        public int Present { get; set; }

        public int Absent { get; set; }

        public int Late { get; set; }

        public int Leave { get; set; }

        public double Percentage { get; set; }
    }
}
