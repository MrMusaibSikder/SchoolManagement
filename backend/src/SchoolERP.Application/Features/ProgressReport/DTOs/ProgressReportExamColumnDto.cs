using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.ProgressReport.DTOs
{
    public class ProgressReportExamColumnDto
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public DateTime? ExamDate { get; set; }
    }
}
