using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.Payment.Interfaces;
using SchoolERP.Domain.Enums;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<SchoolERP.Domain.Entities.Payment>, IPaymentRepository
    {
        public PaymentRepository(SchoolERPDbContext context) : base(context) { }

        public async Task<SchoolERP.Domain.Entities.Payment?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .Include(x => x.Invoice)
                .Include(x => x.Student)
                .Include(x => x.Receipt)
                .Include(x => x.CollectedByEmployee)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IReadOnlyList<SchoolERP.Domain.Entities.Payment>> GetByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .Where(x => x.InvoiceId == invoiceId && x.Status != PaymentStatus.Voided)
                .OrderByDescending(x => x.PaymentDate)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<SchoolERP.Domain.Entities.Payment>> GetByDateRangeAsync(
            DateTime from, DateTime to, int? collectedByEmployeeId = null, CancellationToken cancellationToken = default)
        {
            var query = DbSet.AsNoTracking()
                .Include(x => x.Student)
                .Where(x => x.PaymentDate >= from && x.PaymentDate <= to);

            if (collectedByEmployeeId.HasValue)
                query = query.Where(x => x.CollectedByEmployeeId == collectedByEmployeeId);

            return await query.OrderByDescending(x => x.PaymentDate).ToListAsync(cancellationToken);
        }

        public async Task<string?> GetLastPaymentNumberAsync(CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select(x => x.PaymentNumber)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<bool> TransactionIdExistsAsync(string transactionId, CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .AnyAsync(x => x.TransactionId == transactionId, cancellationToken);

        public async Task<decimal> GetTotalPaidForInvoiceAsync(int invoiceId, CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .Where(x => x.InvoiceId == invoiceId && x.Status == PaymentStatus.Completed)
                .SumAsync(x => x.Amount, cancellationToken);

        public async Task<IReadOnlyList<SchoolERP.Domain.Entities.Payment>> GetCompletedForReportAsync(
    DateTime from, DateTime to, CancellationToken cancellationToken = default)
    => await DbSet.AsNoTracking()
        .Where(x => x.PaymentDate >= from && x.PaymentDate <= to && x.Status == PaymentStatus.Completed)
        .ToListAsync(cancellationToken);
    }
}
