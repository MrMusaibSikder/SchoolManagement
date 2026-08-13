using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.AttendanceReport.DTOs
{
    public class DashboardAttendanceDto
    {
        public int TotalStudents { get; set; }

        public int PresentToday { get; set; }

        public int AbsentToday { get; set; }

        public int LateToday { get; set; }

        public int LeaveToday { get; set; }

        public double AttendancePercentage { get; set; }
    }
}
