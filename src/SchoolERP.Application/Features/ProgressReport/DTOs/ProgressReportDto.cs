using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.ProgressReport.DTOs
{
    public class ProgressReportDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RollNo { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public int AcademicYearId { get; set; }
        public string AcademicYearName { get; set; } = string.Empty;

        public IReadOnlyList<ProgressReportExamColumnDto> Exams { get; set; } = Array.Empty<ProgressReportExamColumnDto>();
        public IReadOnlyList<ProgressReportSubjectRowDto> Subjects { get; set; } = Array.Empty<ProgressReportSubjectRowDto>();
    }
}
