using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Payment.DTOs
{
    /// <summary>
    /// Input model for voiding a payment.
    /// </summary>
    public class VoidPaymentDto
    {
        public string Reason { get; set; } = string.Empty;
    }
}
