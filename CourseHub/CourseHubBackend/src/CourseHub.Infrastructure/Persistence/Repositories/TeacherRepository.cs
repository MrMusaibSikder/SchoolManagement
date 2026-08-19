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
}
