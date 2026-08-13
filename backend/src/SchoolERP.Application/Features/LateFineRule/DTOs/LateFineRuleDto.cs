using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.LateFineRule.DTOs
{
    /// <summary>
    /// Read model returned to clients for a LateFineRule record.
    /// </summary>
    public class LateFineRuleDto
    {
        public int Id { get; set; }
        public int AcademicYearId { get; set; }
        public string AcademicYearName { get; set; } = string.Empty;
        public int? FeeTypeId { get; set; }
        public string? FeeTypeName { get; set; }
        public FineType Type { get; set; }
        public decimal Amount { get; set; }
        public int GracePeriodDays { get; set; }
        public decimal? MaxFineAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
