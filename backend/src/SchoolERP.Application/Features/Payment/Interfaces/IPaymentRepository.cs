using SchoolERP.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Payment.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<SchoolERP.Domain.Entities.Payment>
    {
        Task<SchoolERP.Domain.Entities.Payment?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SchoolERP.Domain.Entities.Payment>> GetByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SchoolERP.Domain.Entities.Payment>> GetByDateRangeAsync(
            DateTime from, DateTime to, int? collectedByEmployeeId = null, CancellationToken cancellationToken = default);
        Task<string?> GetLastPaymentNumberAsync(CancellationToken cancellationToken = default);
        Task<bool> TransactionIdExistsAsync(string transactionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Calculates the total paid amount for the invoice.
        /// Used when recalculating the outstanding balance (BalanceDue).
        /// </summary>
        Task<decimal> GetTotalPaidForInvoiceAsync(int invoiceId, CancellationToken cancellationToken = default);
        
        /// <summary>Completed payments only (Voided/Failed/Refunded বাদ) — collection reports-এর জন্য।</summary>
        Task<IReadOnlyList<SchoolERP.Domain.Entities.Payment>> GetCompletedForReportAsync(
            DateTime from, DateTime to, CancellationToken cancellationToken = default);
    }
}
