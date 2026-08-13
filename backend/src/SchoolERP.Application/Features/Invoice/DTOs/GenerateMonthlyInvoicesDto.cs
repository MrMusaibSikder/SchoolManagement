using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Invoice.DTOs
{
    /// <summary>Input for bulk-generating monthly invoices for every student covered by active fee structures.</summary>
    public class GenerateMonthlyInvoicesDto
    {
        public int AcademicYearId { get; set; }
        public int Month { get; set; }          // 1-12
        public int Year { get; set; }
        /// <summary>Optional — restrict generation to a single class. Null = all classes.</summary>
        public int? SchoolClassId { get; set; }
        /// <summary>Due date applied to every invoice generated in this run.</summary>
        public DateTime DueDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
    }
}
