using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.FeeStructure.DTOs
{
    /// <summary>
    /// Lightweight model used for listing FeeStructures.
    /// </summary>
    public class FeeStructureListDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? AcademicYearName { get; set; }

        public string? SchoolClassName { get; set; }

        public string? SectionName { get; set; }

        public bool IsTemplate { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// Total number of fee items included in this structure.
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// Calculated total amount from all fee structure items.
        /// </summary>
        public decimal TotalAmount { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
