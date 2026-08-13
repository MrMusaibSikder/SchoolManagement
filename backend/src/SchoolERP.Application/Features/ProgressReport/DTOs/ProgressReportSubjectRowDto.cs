using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.ProgressReport.DTOs
{
    public class ProgressReportSubjectRowDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public bool IsOptional { get; set; }
        // index মিলে যাবে Exams লিস্টের index-এর সাথে; পরীক্ষা না দিলে null
        public IReadOnlyList<decimal?> MarksPerExam { get; set; } = Array.Empty<decimal?>();
        public IReadOnlyList<string?> GradePerExam { get; set; } = Array.Empty<string?>();
    }
}
