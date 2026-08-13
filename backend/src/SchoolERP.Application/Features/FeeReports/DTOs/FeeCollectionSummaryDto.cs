using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.FeeReports.DTOs
{
    /// <summary>Aggregate fee collection figures for a date range, with a day-by-day breakdown for trend charts.</summary>
    public class FeeCollectionSummaryDto
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public decimal TotalCollected { get; set; }
        public int TotalTransactions { get; set; }
        public decimal AverageTransactionAmount { get; set; }
        public List<DailyCollectionPointDto> DailyBreakdown { get; set; } = new();
        public List<MethodCollectionPointDto> MethodBreakdown { get; set; } = new();
    }

}
