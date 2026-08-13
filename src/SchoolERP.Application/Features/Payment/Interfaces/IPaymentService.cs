using SchoolERP.Application.Features.Payment.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Payment.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PaymentListDto>> GetByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken = default);
        Task<PaymentDto> CreateAsync(CreatePaymentDto request, CancellationToken cancellationToken = default);
        Task VoidAsync(int id, VoidPaymentDto request, CancellationToken cancellationToken = default);
    }
}
