using SchoolERP.Domain.Common;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Domain.Entities
{
    public class Receipt : BaseEntity
    {
        public string ReceiptNo { get; set; } = string.Empty;
        public int PaymentId { get; set; }
        public Payment Payment { get; set; } = null!;
        public DateTime IssuedAt { get; set; }
        public int IssuedByEmployeeId { get; set; }
        public bool IsVoided { get; set; }
        public DateTime? VoidedAt { get; set; }
        public string? VoidReason { get; set; }
        public Employee IssuedByEmployee { get; set; } = null!;
    }

}
