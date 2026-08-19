using CourseHub.Application.Common.Interfaces;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Persistence.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly CourseHubDbContext _dbContext;

    public CourseRepository(CourseHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Course>> GetPublicListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Courses
            .Where(c => c.IsActive && c.IsPublic)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Courses.CountAsync(c => c.IsActive, cancellationToken);
    }

    public Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Courses.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(string code, Guid? excludingId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Courses.Where(c => c.Code == code);

        if (excludingId.HasValue)
        {
            query = query.Where(c => c.Id != excludingId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Course> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Courses.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim()}%";

            // ILIKE = Postgres case-insensitive LIKE (via Npgsql), so
            // "web" matches "Web Development" without the caller having
            // to worry about casing.
            query = query.Where(c => EF.Functions.ILike(c.Name, term) || EF.Functions.ILike(c.Code, term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Course course, CancellationToken cancellationToken = default)
    {
        await _dbContext.Courses.AddAsync(course, cancellationToken);
    }
}
