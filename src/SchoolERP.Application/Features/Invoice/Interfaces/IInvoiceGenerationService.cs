using SchoolERP.Application.Features.Invoice.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Invoice.Interfaces
{
    public interface IInvoiceGenerationService
    {
        /// <summary>
        /// Generates one invoice per eligible student, per active fee structure covering their
        /// class/section, containing only Monthly-frequency fee items. Skips students who already
        /// have an invoice for the same student/period/fee-structure combination (idempotent — safe
        /// to re-run for the same month; already-invoiced students are simply skipped, not duplicated).
        /// </summary>
        Task<InvoiceGenerationResultDto> GenerateMonthlyInvoicesAsync(
            GenerateMonthlyInvoicesDto request, CancellationToken cancellationToken = default);
    }
}
