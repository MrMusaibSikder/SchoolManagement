using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Domain.Entities
{
    public class LateFineRule : BaseEntity
    {
        public int AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; } = null!;
        public int? FeeTypeId { get; set; }
        public FeeType? FeeType { get; set; }
        public FineType Type { get; set; }
        public decimal Amount { get; set; }
        public int GracePeriodDays { get; set; }
        public decimal? MaxFineAmount { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
