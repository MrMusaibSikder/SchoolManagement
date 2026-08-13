using SchoolERP.Application.Features.Invoice.DTOs;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Invoice.Interfaces
{
    public interface IInvoiceService
    {
        Task<(IReadOnlyList<InvoiceListDto> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, InvoiceStatus? status, int? studentId, int? academicYearId,
            CancellationToken cancellationToken = default);
        Task<InvoiceDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<InvoiceListDto>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
        Task<InvoiceDto> CreateAsync(CreateInvoiceDto request, CancellationToken cancellationToken = default);
        Task<InvoiceDto> CancelAsync(int id, CancelInvoiceDto request, CancellationToken cancellationToken = default);
    }
}
