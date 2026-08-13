using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.SmsLog.DTOs;

namespace SchoolERP.Application.Features.SmsLog.Interfaces;

/// <summary>
/// Business/service contract for SmsLog records. Services return DTOs only
/// and encapsulate all business rules for this feature. Logs are immutable
/// once created (no update operation is exposed); only creation, querying and
/// administrator-only deletion are supported.
/// </summary>
public interface ISmsLogService
{
    /// <summary>Retrieves every SmsLog record.</summary>
    Task<IReadOnlyList<SmsLogDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a search/filtered, paged, sorted list of SmsLog records.</summary>
    Task<PagedResult<SmsLogDto>> GetPagedAsync(SmsLogQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single SmsLog record by id, or null if it does not exist.</summary>
    Task<SmsLogDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new SmsLog record. This is the write path a future SMS gateway
    /// integration will call automatically after attempting delivery.
    /// </summary>
    Task<SmsLogDto> CreateAsync(CreateSmsLogDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing SmsLog record. Intended for administrators only.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Gets aggregate dashboard statistics (today/weekly/monthly/success rate/etc).</summary>
    Task<SmsDashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets logs created today.</summary>
    Task<IReadOnlyList<SmsLogDto>> GetTodayAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets logs created within the last 7 days (including today).</summary>
    Task<IReadOnlyList<SmsLogDto>> GetWeeklyAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets logs with a Failed delivery status, optionally bounded by a date range.</summary>
    Task<IReadOnlyList<SmsLogDto>> GetFailedAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);

    /// <summary>Gets the overall success rate (percentage of Sent/Delivered out of all non-pending logs) for an optional date range.</summary>
    Task<double> GetSuccessRateAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);

    /// <summary>Gets a day-by-day delivery report between two dates.</summary>
    Task<IReadOnlyList<SmsDailyReportDto>> GetDailyReportAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>Gets a month-by-month delivery report for a given year.</summary>
    Task<IReadOnlyList<SmsMonthlyReportDto>> GetMonthlyReportAsync(int year, CancellationToken cancellationToken = default);

    /// <summary>Gets the recipients who received the most SMS messages, optionally bounded by a date range.</summary>
    Task<IReadOnlyList<TopRecipientDto>> GetTopRecipientsAsync(int count, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);

    /// <summary>Gets the most recently created logs, for a "recent activity" feed.</summary>
    Task<IReadOnlyList<SmsLogDto>> GetRecentActivityAsync(int count, CancellationToken cancellationToken = default);
}
