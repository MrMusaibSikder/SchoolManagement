using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.SmsLog.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="SmsLog"/> entities.
/// Works only with the <see cref="SmsLog"/> entity; never returns DTOs.
/// </summary>
public class SmsLogRepository : GenericRepository<SmsLog>, ISmsLogRepository
{
    public SmsLogRepository(SchoolERPDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<SmsLog> Items, int TotalCount)> GetPagedAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = BuildFilteredQuery(searchTerm, status, recipientNumber, studentId, provider, fromDate, toDate);

        var totalCount = await query.CountAsync(cancellationToken);

        query = sortBy?.ToLower() switch
        {
            "recipientnumber" => sortDescending ? query.OrderByDescending(x => x.RecipientNumber) : query.OrderBy(x => x.RecipientNumber),
            "status" => sortDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            "sentat" => sortDescending ? query.OrderByDescending(x => x.SentAt) : query.OrderBy(x => x.SentAt),
            _ => sortDescending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderByDescending(x => x.CreatedAt)
        };

        var items = await query
            .Include(x => x.Student)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SmsLog>> GetBetweenDatesAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var from = fromDate.Date;
        var to = toDate.Date.AddDays(1).AddTicks(-1);

        return await DbSet
            .AsNoTracking()
            .Include(x => x.Student)
            .Where(x => !x.IsDeleted && x.CreatedAt >= from && x.CreatedAt <= to)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(
        SmsStatus? status,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        IQueryable<SmsLog> query = DbSet.AsNoTracking().Where(x => !x.IsDeleted);

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            var to = toDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(x => x.CreatedAt <= to);
        }

        return await query.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SmsLog>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(x => x.Student)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<(string RecipientNumber, int? StudentId, string? StudentName, int MessageCount)>> GetTopRecipientsAsync(
        int count,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        IQueryable<SmsLog> query = DbSet.AsNoTracking().Where(x => !x.IsDeleted);

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            var to = toDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(x => x.CreatedAt <= to);
        }

        var grouped = await query
            .GroupBy(x => new { x.RecipientNumber, x.StudentId })
            .Select(g => new
            {
                g.Key.RecipientNumber,
                g.Key.StudentId,
                MessageCount = g.Count()
            })
            .OrderByDescending(x => x.MessageCount)
            .Take(count)
            .ToListAsync(cancellationToken);

        var studentIds = grouped.Where(x => x.StudentId.HasValue).Select(x => x.StudentId!.Value).ToList();

        var studentNames = await Context.Set<Student>()
            .AsNoTracking()
            .Where(x => studentIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.FullName, cancellationToken);

        return grouped
            .Select(x => (
                x.RecipientNumber,
                x.StudentId,
                x.StudentId.HasValue && studentNames.TryGetValue(x.StudentId.Value, out var name) ? name : null,
                x.MessageCount))
            .ToList();
    }

    /// <summary>Builds the shared filter predicate used by <see cref="GetPagedAsync"/>.</summary>
    private IQueryable<SmsLog> BuildFilteredQuery(
        string? searchTerm,
        SmsStatus? status,
        string? recipientNumber,
        int? studentId,
        string? provider,
        DateTime? fromDate,
        DateTime? toDate)
    {
        IQueryable<SmsLog> query = DbSet.AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(x =>
                x.RecipientNumber.ToLower().Contains(term) ||
                x.Message.ToLower().Contains(term));
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(recipientNumber))
        {
            query = query.Where(x => x.RecipientNumber == recipientNumber);
        }

        if (studentId.HasValue)
        {
            query = query.Where(x => x.StudentId == studentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(provider))
        {
            query = query.Where(x => x.Provider == provider);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            var to = toDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(x => x.CreatedAt <= to);
        }

        return query;
    }
}
