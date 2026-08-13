using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Invoice.DTOs
{
    /// <summary>
    /// Input model for creating a new Invoice.
    /// </summary>
    public class CreateInvoiceDto
    {
        public int StudentId { get; set; }

        public int AcademicYearId { get; set; }

        public int? FeeStructureId { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }

        public int? Month { get; set; }

        public int? Year { get; set; }

        public string? Notes { get; set; }

        public List<CreateInvoiceItemDto> Items { get; set; } = new();
    }
    /// <summary>
    /// Input model for adding invoice line items.
    /// </summary>
    public class CreateInvoiceItemDto
    {
        public int FeeTypeId { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal OriginalAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal FineAmount { get; set; }

        public int Quantity { get; set; } = 1;

        public int SortOrder { get; set; }
    }
}
