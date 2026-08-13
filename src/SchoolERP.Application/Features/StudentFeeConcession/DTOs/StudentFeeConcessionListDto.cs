using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.StudentFeeConcession.DTOs
{
    public class StudentFeeConcessionListDto
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string FeeTypeName { get; set; } = string.Empty;
        public string AcademicYearName { get; set; } = string.Empty;
        
        public ConcessionType Type { get; set; }
        public decimal? Value { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
    }
}
