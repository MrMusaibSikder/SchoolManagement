using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Payment.DTOs
{
    /// <summary>
    /// Read model returned for payment details.
    /// </summary>
    public class PaymentDto
    {
        public int Id { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string? TransactionId { get; set; }
        public string? Remarks { get; set; }
        public int CollectedByEmployeeId { get; set; }
        public string CollectedByEmployeeName { get; set; } = string.Empty;
        public int? ReceiptId { get; set; }
        public string? ReceiptNo { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
