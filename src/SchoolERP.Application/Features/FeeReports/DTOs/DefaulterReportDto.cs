using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.FeeReports.DTOs
{
    /// <summary>List of students with overdue, unpaid invoice balances, as of a given date.</summary>
    public class DefaulterReportDto
    {
        public DateTime AsOfDate { get; set; }
        public int TotalDefaulters { get; set; }
        public decimal TotalOverdueAmount { get; set; }
        public List<DefaulterDto> Defaulters { get; set; } = new();
    }

    public class DefaulterDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string AdmissionNumber { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string? SectionName { get; set; }
        public int OverdueInvoiceCount { get; set; }
        public decimal TotalOverdueAmount { get; set; }
        public DateTime OldestDueDate { get; set; }
        public int DaysOverdue { get; set; }
    }
}
