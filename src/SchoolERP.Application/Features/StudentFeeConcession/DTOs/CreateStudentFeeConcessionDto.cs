using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.StudentFeeConcession.DTOs
{
    /// <summary>
    /// Input model for creating a new StudentFeeConcession record.
    /// </summary>
    public class CreateStudentFeeConcessionDto
    {
        public int StudentId { get; set; }
        public int FeeTypeId { get; set; }
        public int AcademicYearId { get; set; }
        public ConcessionType Type { get; set; }
        public decimal? Value { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool RequiresApproval { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
