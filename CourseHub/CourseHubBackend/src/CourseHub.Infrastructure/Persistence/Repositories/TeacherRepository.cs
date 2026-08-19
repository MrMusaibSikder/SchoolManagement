using CourseHub.Application.Common.Interfaces;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Persistence.Repositories;

public class TeacherRepository : ITeacherRepository
{
    private readonly CourseHubDbContext _dbContext;

    public TeacherRepository(CourseHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Teacher>> GetPublicListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Teachers
            .Where(t => t.IsActive && t.IsProfilePublic)
            .OrderBy(t => t.FirstName)
            .ThenBy(t => t.LastName)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Teachers.CountAsync(t => t.IsActive, cancellationToken);
    }

    public Task<Teacher?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Teachers.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Teachers.AnyAsync(t => t.UserId == userId, cancellationToken);
    }

    public Task<bool> ExistsByEmployeeIdAsync(string employeeId, Guid? excludingId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Teachers.Where(t => t.EmployeeId == employeeId);

        if (excludingId.HasValue)
        {
            query = query.Where(t => t.Id != excludingId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Teacher> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Teachers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim()}%";

            query = query.Where(t =>
                EF.Functions.ILike(t.FirstName, term) ||
                EF.Functions.ILike(t.LastName, term) ||
                EF.Functions.ILike(t.EmployeeId, term) ||
                (t.Email != null && EF.Functions.ILike(t.Email, term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(t => t.FirstName)
            .ThenBy(t => t.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Teacher teacher, CancellationToken cancellationToken = default)
    {
        await _dbContext.Teachers.AddAsync(teacher, cancellationToken);
    }
}
