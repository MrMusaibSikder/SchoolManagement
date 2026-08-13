using SchoolERP.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Domain.Entities
{
    public class InvoiceItem : BaseEntity
    {
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;

        public int FeeTypeId { get; set; }
        public FeeType FeeType { get; set; } = null!;

        public string Description { get; set; } = string.Empty;
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FineAmount { get; set; }
        public decimal NetAmount { get; set; }

        public int Quantity { get; set; } = 1;
        public int SortOrder { get; set; }
    }
}
