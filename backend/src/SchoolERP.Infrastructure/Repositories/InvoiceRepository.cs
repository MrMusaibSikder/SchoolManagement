using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.Invoice.Interfaces;
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
    public class InvoiceRepository : GenericRepository<SchoolERP.Domain.Entities.Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(SchoolERPDbContext context) : base(context) { }

        public async Task<SchoolERP.Domain.Entities.Invoice?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .Include(x => x.AcademicYear)
                .Include(x => x.Student)
                .Include(x => x.InvoiceItems).ThenInclude(i => i.FeeType)
                .Include(x => x.CancelledByEmployee)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken); // Note: no IsDeleted filter — Invoice has no soft delete flag active by default; adjust if you add HasQueryFilter

        public async Task<SchoolERP.Domain.Entities.Invoice?> GetTrackedWithItemsAsync(int id, CancellationToken cancellationToken = default)
            => await DbSet
                .Include(x => x.InvoiceItems)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IReadOnlyList<SchoolERP.Domain.Entities.Invoice>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .Where(x => x.StudentId == studentId)
                .OrderByDescending(x => x.InvoiceDate)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<SchoolERP.Domain.Entities.Invoice>> GetOverdueInvoicesAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
            => await DbSet
                .Where(x => x.DueDate < asOfDate
                         && x.Status != InvoiceStatus.Paid
                         && x.Status != InvoiceStatus.Cancelled
                         && x.BalanceDue > 0)
                .ToListAsync(cancellationToken);

        public async Task<bool> ExistsForPeriodAsync(
            int studentId, int academicYearId, int? month, int? year, int feeStructureId, CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .AnyAsync(x => x.StudentId == studentId
                            && x.AcademicYearId == academicYearId
                            && x.Month == month
                            && x.Year == year
                            && x.FeeStructureId == feeStructureId, cancellationToken);

        public async Task<string?> GetLastInvoiceNumberAsync(int academicYearId, CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .Where(x => x.AcademicYearId == academicYearId)
                .OrderByDescending(x => x.Id)
                .Select(x => x.InvoiceNumber)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<(IReadOnlyList<SchoolERP.Domain.Entities.Invoice> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, InvoiceStatus? status, int? studentId, int? academicYearId,
            CancellationToken cancellationToken = default)
        {
            var query = DbSet.AsNoTracking()
                .Include(x => x.Student)
                .AsQueryable();

            if (status.HasValue) query = query.Where(x => x.Status == status);
            if (studentId.HasValue) query = query.Where(x => x.StudentId == studentId);
            if (academicYearId.HasValue) query = query.Where(x => x.AcademicYearId == academicYearId);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.InvoiceDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<IReadOnlyList<SchoolERP.Domain.Entities.Invoice>> GetOverdueWithStudentDetailsAsync(
    DateTime asOfDate, CancellationToken cancellationToken = default)
    => await DbSet.AsNoTracking()
        .Include(x => x.Student).ThenInclude(s => s.SchoolClass)
        .Include(x => x.Student).ThenInclude(s => s.Section)
        .Where(x => x.DueDate < asOfDate
                 && x.Status != InvoiceStatus.Paid
                 && x.Status != InvoiceStatus.Cancelled
                 && x.BalanceDue > 0)
        .ToListAsync(cancellationToken);
    }
}
