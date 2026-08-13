using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.AttendanceReport.DTOs
{
    public class AttendanceTrendDto
    {
        public DateTime Date { get; set; }

        public int Present { get; set; }

        public int Absent { get; set; }

        public int Late { get; set; }

        public int Leave { get; set; }
    }
}
