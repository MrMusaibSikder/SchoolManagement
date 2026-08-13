using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Invoice.DTOs
{
    /// <summary>
    /// Input model for cancelling an invoice.
    /// </summary>
    public class CancelInvoiceDto
    {
        public string CancellationReason { get; set; } = string.Empty;
    }
}
