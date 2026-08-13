using SchoolERP.Application.Features.FeeReports.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.FeeReports.Interfaces
{
    public interface IFeeReportService
    {
        Task<FeeCollectionSummaryDto> GetCollectionSummaryAsync(
            DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default);

        Task<DefaulterReportDto> GetDefaulterReportAsync(
            DateTime? asOfDate = null, int? schoolClassId = null, CancellationToken cancellationToken = default);
    }
}
