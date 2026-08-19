using CourseHub.Application.Common.Interfaces;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Persistence.Repositories;

public class BatchRepository : IBatchRepository
{
    private readonly CourseHubDbContext _dbContext;

    public BatchRepository(CourseHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Batches.CountAsync(b => b.IsActive, cancellationToken);
    }

    public Task<Batch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Batches.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(string code, Guid? excludingId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Batches.Where(b => b.Code == code);

        if (excludingId.HasValue)
        {
            query = query.Where(b => b.Id != excludingId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Batch> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        Guid? courseId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Batches.AsQueryable();

        if (courseId.HasValue)
        {
            query = query.Where(b => b.CourseId == courseId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim()}%";

            query = query.Where(b => EF.Functions.ILike(b.Name, term) || EF.Functions.ILike(b.Code, term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(b => b.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Batch batch, CancellationToken cancellationToken = default)
    {
        await _dbContext.Batches.AddAsync(batch, cancellationToken);
    }
}
