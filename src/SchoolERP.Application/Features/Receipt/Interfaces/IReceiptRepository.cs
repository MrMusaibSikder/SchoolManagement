using SchoolERP.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Receipt.Interfaces
{
    public interface IReceiptRepository : IGenericRepository<SchoolERP.Domain.Entities.Receipt>
    {
        Task<SchoolERP.Domain.Entities.Receipt?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<SchoolERP.Domain.Entities.Receipt?> GetByPaymentIdAsync(int paymentId, CancellationToken cancellationToken = default);
        Task<string?> GetLastReceiptNoAsync(CancellationToken cancellationToken = default);
    }
}
