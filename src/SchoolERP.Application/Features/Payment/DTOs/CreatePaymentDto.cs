using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Payment.DTOs
{
    /// <summary>
    /// Input model for recording a new payment.
    /// </summary>
    public class CreatePaymentDto
    {
        public int InvoiceId { get; set; }
        public int StudentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod Method { get; set; }
        public string? TransactionId { get; set; }
        public string? Remarks { get; set; }
    }
}
