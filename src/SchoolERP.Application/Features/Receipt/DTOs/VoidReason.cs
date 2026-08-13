using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Receipt.DTOs
{
    /// <summary>
    /// Input model for voiding a Receipt.
    /// </summary>
    public class VoidReceiptDto
    {
        public string VoidReason { get; set; } = string.Empty;
    }
}
