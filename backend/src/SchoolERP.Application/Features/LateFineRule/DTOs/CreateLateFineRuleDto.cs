using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.LateFineRule.DTOs
{
    /// <summary>
    /// Input model for creating a new LateFineRule record.
    /// </summary>
    public class CreateLateFineRuleDto
    {
        public int AcademicYearId { get; set; }
        public int? FeeTypeId { get; set; }
        public FineType Type { get; set; }
        public decimal Amount { get; set; }
        public int GracePeriodDays { get; set; }
        public decimal? MaxFineAmount { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
