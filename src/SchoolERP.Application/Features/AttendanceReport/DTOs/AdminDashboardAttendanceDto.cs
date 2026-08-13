using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.AttendanceReport.DTOs
{
    public class AdminDashboardAttendanceDto
    {
        public int TotalStudents { get; set; }

        public int TotalPresent { get; set; }

        public int TotalAbsent { get; set; }

        public int TotalLate { get; set; }

        public int TotalLeave { get; set; }

        public double AttendancePercentage { get; set; }
    }
}
