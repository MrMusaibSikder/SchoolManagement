using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.Notice.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="Notice"/> entities.
/// Works only with the <see cref="Notice"/> entity; never returns DTOs.
/// Contains database queries only — publish/archive transitions, dashboard
/// aggregation and every other business rule live in <c>NoticeService</c>.
/// </summary>
public class NoticeRepository : GenericRepository<Notice>, INoticeRepository
{
    public NoticeRepository(SchoolERPDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<Notice> Items, int TotalCount)> GetPagedAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = BuildFilteredQuery(searchTerm, audience, priority, isPublished, isArchived, fromDate, toDate);

        var totalCount = await query.CountAsync(cancellationToken);

        query = sortBy?.ToLower() switch
        {
            "title" => sortDescending ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title),
            "priority" => sortDescending ? query.OrderByDescending(x => x.Priority) : query.OrderBy(x => x.Priority),
            "expirydate" => sortDescending ? query.OrderByDescending(x => x.ExpiryDate) : query.OrderBy(x => x.ExpiryDate),
            "publishdate" => sortDescending ? query.OrderByDescending(x => x.PublishDate) : query.OrderBy(x => x.PublishDate),
            _ => sortDescending ? query.OrderByDescending(x => x.PublishDate) : query.OrderByDescending(x => x.PublishDate)
        };

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Notice>> GetActiveAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
    {
        var date = asOfDate.Date;

        return await DbSet
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.IsPublished &&
                !x.IsArchived &&
                x.PublishDate.Date <= date &&
                (!x.ExpiryDate.HasValue || x.ExpiryDate.Value.Date >= date))
            .OrderByDescending(x => x.PublishDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Notice>> GetUpcomingAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
    {
        var date = asOfDate.Date;

        return await DbSet
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                !x.IsArchived &&
                x.PublishDate.Date > date)
            .OrderBy(x => x.PublishDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Notice>> GetExpiredAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
    {
        var date = asOfDate.Date;

        return await DbSet
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                !x.IsArchived &&
                x.ExpiryDate.HasValue &&
                x.ExpiryDate.Value.Date < date)
            .OrderByDescending(x => x.ExpiryDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Notice>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.PublishDate)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Notice>> GetHighPriorityAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                !x.IsArchived &&
                x.IsPublished &&
                x.Priority == NoticePriority.High)
            .OrderByDescending(x => x.PublishDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountByStateAsync(bool? isPublished, bool? isArchived, CancellationToken cancellationToken = default)
    {
        IQueryable<Notice> query = DbSet.AsNoTracking().Where(x => !x.IsDeleted);

        if (isPublished.HasValue)
        {
            query = query.Where(x => x.IsPublished == isPublished.Value);
        }

        if (isArchived.HasValue)
        {
            query = query.Where(x => x.IsArchived == isArchived.Value);
        }

        return await query.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> TitleExistsAsync(string title, int? excludeId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(x =>
                !x.IsDeleted &&
                x.Title.ToLower() == title.ToLower() &&
                (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
    }

    /// <summary>Builds the shared filter predicate used by <see cref="GetPagedAsync"/>.</summary>
    private IQueryable<Notice> BuildFilteredQuery(
        string? searchTerm,
        NoticeAudience? audience,
        NoticePriority? priority,
        bool? isPublished,
        bool? isArchived,
        DateTime? fromDate,
        DateTime? toDate)
    {
        IQueryable<Notice> query = DbSet.AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(x =>
                x.Title.ToLower().Contains(term) ||
                x.Description.ToLower().Contains(term));
        }

        if (audience.HasValue)
        {
            query = query.Where(x => x.Audience == audience.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(x => x.Priority == priority.Value);
        }

        if (isPublished.HasValue)
        {
            query = query.Where(x => x.IsPublished == isPublished.Value);
        }

        if (isArchived.HasValue)
        {
            query = query.Where(x => x.IsArchived == isArchived.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.PublishDate.Date >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x => x.PublishDate.Date <= toDate.Value.Date);
        }

        return query;
    }

    public async Task<IReadOnlyList<Notice>> GetPublicPublishedAsync(
    int take, CancellationToken cancellationToken = default)
    => await DbSet.AsNoTracking()
        .Where(x => !x.IsDeleted
                 && x.IsPublished
                 && !x.IsArchived                                    // আগে ভুলে বাদ পড়েছিল
                 && x.Audience == NoticeAudience.Everyone
                 && (x.ExpiryDate == null || x.ExpiryDate >= DateTime.UtcNow))  // expired notice বাদ
        .OrderByDescending(x => x.PublishDate)                       //  PublishedAt → PublishDate
        .Take(take)
        .ToListAsync(cancellationToken);
}
