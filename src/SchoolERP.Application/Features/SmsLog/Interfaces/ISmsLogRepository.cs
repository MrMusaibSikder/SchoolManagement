using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.SmsLog.Interfaces;

/// <summary>
/// Repository contract for <see cref="SchoolERP.Domain.Entities.SmsLog"/> entities.
/// Extends the generic repository with SmsLog-specific data access members.
/// Contains database operations only; all aggregation/reporting business rules
/// live in <c>ISmsLogService</c>.
/// </summary>
public interface ISmsLogRepository : IGenericRepository<SchoolERP.Domain.Entities.SmsLog>
{
    /// <summary>
    /// Retrieves a search/filtered, sorted page of logs (with the related Student
    /// eagerly loaded), along with the total number of matching records.
    /// </summary>
    Task<(IReadOnlyList<SchoolERP.Domain.Entities.SmsLog> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        SmsStatus? status,
        string? recipientNumber,
        int? studentId,
        string? provider,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool sortDescending,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every log created between two dates (inclusive), with the related Student eagerly loaded.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.SmsLog>> GetBetweenDatesAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    /// <summary>Counts logs matching an optional status and date range.</summary>
    Task<int> CountAsync(
        SmsStatus? status,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    /// <summary>Gets the most recently created logs, with the related Student eagerly loaded.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.SmsLog>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the recipient numbers that received the most SMS messages within an
    /// optional date range, ordered by message count descending.
    /// </summary>
    Task<IReadOnlyList<(string RecipientNumber, int? StudentId, string? StudentName, int MessageCount)>> GetTopRecipientsAsync(
        int count,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);
}
