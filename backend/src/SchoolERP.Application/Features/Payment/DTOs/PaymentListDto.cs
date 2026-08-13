using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Payment.DTOs
{
    /// <summary>
    /// Lightweight model for payment listing.
    /// </summary>
    public class PaymentListDto
    {
        public int Id { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string? ReceiptNo { get; set; }
    }
}
