using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Invoice.DTOs
{
    public class LateFineApplicationResultDto
    {
        public int TotalInvoicesEvaluated { get; set; }
        public int InvoicesUpdated { get; set; }
        public int SkippedWithinGracePeriod { get; set; }
        public int SkippedNoRule { get; set; }
        public int Failed { get; set; }
        public decimal TotalFineApplied { get; set; }
        public List<InvoiceGenerationErrorDto> Errors { get; set; } = new(); //  (StudentId/Name/Reason shape )
    }
}
