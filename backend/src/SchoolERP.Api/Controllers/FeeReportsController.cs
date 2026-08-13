using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.FeeReports.DTOs;
using SchoolERP.Application.Features.FeeReports.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/fee-reports")]
    [Produces("application/json")]
    [ApiController]
    public class FeeReportsController : ControllerBase
    {
        private readonly IFeeReportService _feeReportService;

        public FeeReportsController(IFeeReportService feeReportService)
        {
            _feeReportService = feeReportService;
        }

        /// <summary>Fee collection totals for a date range, with daily and payment-method breakdowns.</summary>
        [HttpGet("collection-summary")]
        [PermissionAuthorize(PermissionNames.FeeReportView)]
        [ProducesResponseType(typeof(FeeCollectionSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FeeCollectionSummaryDto>> GetCollectionSummary(
            [FromQuery] DateTime dateFrom,
            [FromQuery] DateTime dateTo,
            CancellationToken cancellationToken)
        {
            return Ok(await _feeReportService.GetCollectionSummaryAsync(dateFrom, dateTo, cancellationToken));
        }

        /// <summary>Students with overdue, unpaid invoice balances — optionally filtered by class.</summary>
        [HttpGet("defaulters")]
        [PermissionAuthorize(PermissionNames.FeeReportView)]
        [ProducesResponseType(typeof(DefaulterReportDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DefaulterReportDto>> GetDefaulters(
            [FromQuery] DateTime? asOfDate,
            [FromQuery] int? schoolClassId,
            CancellationToken cancellationToken)
        {
            return Ok(await _feeReportService.GetDefaulterReportAsync(asOfDate, schoolClassId, cancellationToken));
        }
    }
}
