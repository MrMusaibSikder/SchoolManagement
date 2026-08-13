using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.SmsLog.DTOs;
using SchoolERP.Application.Features.SmsLog.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Read-only SMS delivery log querying, dashboard statistics and reporting.
    /// Logs cannot be edited once created; only creation (for system/gateway
    /// use), querying, and administrator-only deletion are exposed.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SmsLogController : ControllerBase
    {
        private readonly ISmsLogService _smsLogService;

        /// <summary>Initializes a new instance of <see cref="SmsLogController"/>.</summary>
        public SmsLogController(ISmsLogService smsLogService)
        {
            _smsLogService = smsLogService;
        }

        /// <summary>
        /// Get a search-filtered, paged, sorted list of SMS logs. Supports
        /// filtering by status, recipient, student, provider and date range.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.SmsLogView)]
        [ProducesResponseType(typeof(PagedResult<SmsLogDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<SmsLogDto>>> GetPaged(
            [FromQuery] SmsLogQueryDto query,
            CancellationToken cancellationToken)
        {
            var result = await _smsLogService.GetPagedAsync(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get an SMS log by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.SmsLogView)]
        [ProducesResponseType(typeof(SmsLogDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SmsLogDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _smsLogService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Create a new SMS log entry. Intended primarily for system/gateway use
        /// once a real SMS provider integration is wired up; logs cannot be
        /// edited afterwards.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.SmsLogCreate)]
        [ProducesResponseType(typeof(SmsLogDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SmsLogDto>> Create(
            [FromBody] CreateSmsLogDto request,
            CancellationToken cancellationToken)
        {
            var result = await _smsLogService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Delete an SMS log. Administrator-only; logs are otherwise permanent.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.SmsLogDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _smsLogService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Get aggregate dashboard statistics: total/today/weekly/monthly counts,
        /// success/failed/pending counts and success rate.
        /// </summary>
        [HttpGet("dashboard")]
        [PermissionAuthorize(PermissionNames.SmsLogView)]
        [ProducesResponseType(typeof(SmsDashboardStatsDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SmsDashboardStatsDto>> GetDashboard(CancellationToken cancellationToken)
        {
            var result = await _smsLogService.GetDashboardStatsAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get every SMS log created today.
        /// </summary>
        [HttpGet("today")]
        [PermissionAuthorize(PermissionNames.SmsLogView)]
        [ProducesResponseType(typeof(IReadOnlyList<SmsLogDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SmsLogDto>>> GetToday(CancellationToken cancellationToken)
        {
            var result = await _smsLogService.GetTodayAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get every SMS log created within the last 7 days.
        /// </summary>
        [HttpGet("weekly")]
        [PermissionAuthorize(PermissionNames.SmsLogView)]
        [ProducesResponseType(typeof(IReadOnlyList<SmsLogDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SmsLogDto>>> GetWeekly(CancellationToken cancellationToken)
        {
            var result = await _smsLogService.GetWeeklyAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get failed SMS logs, optionally bounded by a date range.
        /// </summary>
        [HttpGet("failed")]
        [PermissionAuthorize(PermissionNames.SmsLogView)]
        [ProducesResponseType(typeof(IReadOnlyList<SmsLogDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SmsLogDto>>> GetFailed(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var result = await _smsLogService.GetFailedAsync(fromDate, toDate, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the overall SMS success rate (percentage), optionally bounded by a
        /// date range.
        /// </summary>
        [HttpGet("success-rate")]
        [PermissionAuthorize(PermissionNames.SmsLogView)]
        [ProducesResponseType(typeof(double), StatusCodes.Status200OK)]
        public async Task<ActionResult<double>> GetSuccessRate(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var result = await _smsLogService.GetSuccessRateAsync(fromDate, toDate, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a day-by-day SMS delivery report between two dates.
        /// </summary>
        [HttpGet("daily-report")]
        [PermissionAuthorize(PermissionNames.SmsLogView)]
        [ProducesResponseType(typeof(IReadOnlyList<SmsDailyReportDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SmsDailyReportDto>>> GetDailyReport(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            CancellationToken cancellationToken)
        {
            var result = await _smsLogService.GetDailyReportAsync(fromDate, toDate, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a month-by-month SMS delivery report for a given year.
        /// </summary>
        [HttpGet("monthly-report")]
        [PermissionAuthorize(PermissionNames.SmsLogView)]
        [ProducesResponseType(typeof(IReadOnlyList<SmsMonthlyReportDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SmsMonthlyReportDto>>> GetMonthlyReport(
            [FromQuery] int year,
            CancellationToken cancellationToken)
        {
            var result = await _smsLogService.GetMonthlyReportAsync(year, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the recipients who received the most SMS messages, optionally
        /// bounded by a date range.
        /// </summary>
        [HttpGet("top-recipients")]
        [PermissionAuthorize(PermissionNames.SmsLogView)]
        [ProducesResponseType(typeof(IReadOnlyList<TopRecipientDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<TopRecipientDto>>> GetTopRecipients(
            [FromQuery] int count = 10,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _smsLogService.GetTopRecipientsAsync(count, fromDate, toDate, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the most recently created SMS logs, for a "recent activity" feed.
        /// </summary>
        [HttpGet("recent")]
        [PermissionAuthorize(PermissionNames.SmsLogView)]
        [ProducesResponseType(typeof(IReadOnlyList<SmsLogDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SmsLogDto>>> GetRecent(
            [FromQuery] int count = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _smsLogService.GetRecentActivityAsync(count, cancellationToken);
            return Ok(result);
        }
    }
}
