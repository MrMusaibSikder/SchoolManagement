using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Receipt.Interfaces
{
    public interface IReceiptPdfService
    {
        /// <summary>Generates a printable PDF for the given receipt. Throws NotFoundException if the receipt doesn't exist.</summary>
        Task<byte[]> GenerateReceiptPdfAsync(int receiptId, CancellationToken cancellationToken = default);
    }
}
