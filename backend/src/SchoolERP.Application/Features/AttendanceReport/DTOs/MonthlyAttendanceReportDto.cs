using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.AttendanceReport.DTOs
{
    public class MonthlyAttendanceReportDto
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public int TotalSchoolDays { get; set; }

        public int Present { get; set; }

        public int Absent { get; set; }

        public int Late { get; set; }

        public int Leave { get; set; }

        public double Percentage { get; set; }
    }
}
