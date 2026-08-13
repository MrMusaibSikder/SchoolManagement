using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public string PaymentNumber { get; set; } = string.Empty;
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Completed;
        public string? TransactionId { get; set; }
        public string? Remarks { get; set; }
        public int CollectedByEmployeeId { get; set; }
        public Receipt? Receipt { get; set; }
        public Employee CollectedByEmployee { get; set; } = null!;
    }
}
