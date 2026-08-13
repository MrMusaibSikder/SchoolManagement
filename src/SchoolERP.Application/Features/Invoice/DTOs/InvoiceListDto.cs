using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Invoice.DTOs
{
    /// <summary>
    /// Lightweight invoice model for grid/list view.
    /// </summary>
    public class InvoiceListDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public InvoiceStatus Status { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal BalanceDue { get; set; }
    }
}
