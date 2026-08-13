using SchoolERP.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Domain.Entities
{
    public class FeeStructureItem : BaseEntity
    {
        public int FeeStructureId { get; set; }
        public FeeStructure FeeStructure { get; set; } = null!;

        public int FeeTypeId { get; set; }
        public FeeType FeeType { get; set; } = null!;

        public decimal Amount { get; set; }
        public bool IsOptional { get; set; }
        public int SortOrder { get; set; }
    }
}
