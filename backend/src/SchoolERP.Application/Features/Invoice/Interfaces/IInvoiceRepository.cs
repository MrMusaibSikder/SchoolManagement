using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Invoice.Interfaces
{
    public interface IInvoiceRepository : IGenericRepository<SchoolERP.Domain.Entities.Invoice>
    {
        Task<SchoolERP.Domain.Entities.Invoice?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the tracked entity before updating the Status or Amount to ensure RowVersion concurrency is handled correctly.
        /// </summary>
        Task<SchoolERP.Domain.Entities.Invoice?> GetTrackedWithItemsAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<SchoolERP.Domain.Entities.Invoice>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<SchoolERP.Domain.Entities.Invoice>> GetOverdueInvoicesAsync(DateTime asOfDate, CancellationToken cancellationToken = default);

        Task<bool> ExistsForPeriodAsync(
            int studentId, int academicYearId, int? month, int? year, int feeStructureId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the latest invoice number as raw data to generate the next invoice number.
        /// The actual formatting is handled in the service layer.
        /// </summary>
        
        Task<string?> GetLastInvoiceNumberAsync(int academicYearId, CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<SchoolERP.Domain.Entities.Invoice> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, InvoiceStatus? status, int? studentId, int? academicYearId,
            CancellationToken cancellationToken = default);

        /// <summary>Overdue invoice + Student/Class/Section detail  — Defaulter report-এর জন্য।</summary>
        Task<IReadOnlyList<SchoolERP.Domain.Entities.Invoice>> GetOverdueWithStudentDetailsAsync(
            DateTime asOfDate, CancellationToken cancellationToken = default);
    }
}
