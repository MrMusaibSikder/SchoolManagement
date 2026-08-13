using SchoolERP.Application.Features.Receipt.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Receipt.Interfaces
{
    public interface IReceiptService
    {
        Task<ReceiptDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ReceiptDto?> GetByPaymentIdAsync(int paymentId, CancellationToken cancellationToken = default);
        Task VoidAsync(int id, VoidReceiptDto request, CancellationToken cancellationToken = default);
    }
}
