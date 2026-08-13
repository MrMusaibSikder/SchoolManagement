using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Domain.Entities
{
    public class StudentFeeConcession : BaseEntity
    {
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public int FeeTypeId { get; set; }
        public FeeType FeeType { get; set; } = null!;
        public int AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; } = null!;
        public ConcessionType Type { get; set; }
        public decimal? Value { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool RequiresApproval { get; set; }
        public bool IsApproved { get; set; }
        public int? ApprovedByEmployeeId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; } = true;
        public Employee? ApprovedByEmployee { get; set; }
    }
}
