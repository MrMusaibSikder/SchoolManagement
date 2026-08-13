using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Receipt.DTOs
{
    /// <summary>
    /// Read model returned to clients for a Receipt record.
    /// </summary>
    public class ReceiptDto
    {
        public int Id { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public int PaymentId { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; }
        public int IssuedByEmployeeId { get; set; }
        public string IssuedByEmployeeName { get; set; } = string.Empty;
        public bool IsVoided { get; set; }
        public DateTime? VoidedAt { get; set; }
        public string? VoidReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
