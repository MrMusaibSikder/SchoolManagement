using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Notice.Interfaces;

/// <summary>
/// Repository contract for <see cref="SchoolERP.Domain.Entities.Notice"/> entities.
/// Extends the generic repository with Notice-specific data access members.
/// Contains database operations only; all business rules (publish/archive
/// transitions, dashboard aggregation, etc.) live in <c>INoticeService</c>.
/// </summary>
public interface INoticeRepository : IGenericRepository<SchoolERP.Domain.Entities.Notice>
{
    /// <summary>
    /// Retrieves a search-filtered, sorted page of notices, along with the total
    /// number of matching records (before paging).
    /// </summary>
    Task<(IReadOnlyList<SchoolERP.Domain.Entities.Notice> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        NoticeAudience? audience,
        NoticePriority? priority,
        bool? isPublished,
        bool? isArchived,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool sortDescending,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets every notice that is currently active: published, not archived, and
    /// either has no expiry date or has not yet expired as of <paramref name="asOfDate"/>.
    /// </summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Notice>> GetActiveAsync(
        DateTime asOfDate,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every notice whose publish date is still in the future as of <paramref name="asOfDate"/>.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Notice>> GetUpcomingAsync(
        DateTime asOfDate,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every non-archived notice whose expiry date has passed as of <paramref name="asOfDate"/>.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Notice>> GetExpiredAsync(
        DateTime asOfDate,
        CancellationToken cancellationToken = default);

    /// <summary>Gets the most recently published notices.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Notice>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every published, non-archived, high priority notice.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Notice>> GetHighPriorityAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Counts notices matching optional published/archived state, used by the dashboard summary.</summary>
    Task<int> CountByStateAsync(
        bool? isPublished,
        bool? isArchived,
        CancellationToken cancellationToken = default);

    /// <summary>Checks whether another (non-deleted) notice already has the same title.</summary>
    Task<bool> TitleExistsAsync(
        string title,
        int? excludeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SchoolERP.Domain.Entities.Notice>> GetPublicPublishedAsync(
    int take, CancellationToken cancellationToken = default);
}
