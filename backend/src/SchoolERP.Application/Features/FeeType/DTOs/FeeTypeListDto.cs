using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.FeeType.DTOs
{
    /// <summary>
    /// Lightweight model used when listing FeeTypes.
    /// </summary>
    public class FeeTypeListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? FeeCategoryName { get; set; }
        public FeeFrequency Frequency { get; set; }
        public bool IsActive { get; set; }
    }
}
