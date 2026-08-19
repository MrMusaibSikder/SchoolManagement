using CourseHub.Application.Common.Interfaces;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Persistence.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly CourseHubDbContext _dbContext;

    public StudentRepository(CourseHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Students.CountAsync(s => s.IsActive, cancellationToken);
    }

    public Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Students.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Students.AnyAsync(s => s.UserId == userId, cancellationToken);
    }

    public Task<bool> ExistsByStudentIdAsync(string studentId, Guid? excludingId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Students.Where(s => s.StudentId == studentId);

        if (excludingId.HasValue)
        {
            query = query.Where(s => s.Id != excludingId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Student> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Students.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim()}%";

            query = query.Where(s =>
                EF.Functions.ILike(s.FirstName, term) ||
                EF.Functions.ILike(s.LastName, term) ||
                EF.Functions.ILike(s.StudentId, term) ||
                (s.Email != null && EF.Functions.ILike(s.Email, term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.FirstName)
            .ThenBy(s => s.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Student student, CancellationToken cancellationToken = default)
    {
        await _dbContext.Students.AddAsync(student, cancellationToken);
    }
}
