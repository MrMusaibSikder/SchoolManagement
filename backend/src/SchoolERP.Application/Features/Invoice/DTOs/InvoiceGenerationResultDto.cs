using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Invoice.DTOs
{
    /// <summary>Summary + per-student error detail returned after a bulk generation run.</summary>
    public class InvoiceGenerationResultDto
    {
        public int TotalStudentsEvaluated { get; set; }
        public int InvoicesCreated { get; set; }
        public int SkippedAlreadyInvoiced { get; set; }
        public int SkippedNoMonthlyItems { get; set; }
        public int Failed { get; set; }
        public List<InvoiceGenerationErrorDto> Errors { get; set; } = new();
    }
    public class InvoiceGenerationErrorDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }

}
