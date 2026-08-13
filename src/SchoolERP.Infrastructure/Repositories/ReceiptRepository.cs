using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.Receipt.Interfaces;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Repositories
{
    public class ReceiptRepository : GenericRepository<SchoolERP.Domain.Entities.Receipt>, IReceiptRepository
    {
        public ReceiptRepository(SchoolERPDbContext context) : base(context) { }

        public async Task<SchoolERP.Domain.Entities.Receipt?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .Include(x => x.Payment)
                .Include(x => x.IssuedByEmployee)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<SchoolERP.Domain.Entities.Receipt?> GetByPaymentIdAsync(int paymentId, CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.PaymentId == paymentId, cancellationToken);

        public async Task<string?> GetLastReceiptNoAsync(CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select(x => x.ReceiptNo)
                .FirstOrDefaultAsync(cancellationToken);
    }
}
