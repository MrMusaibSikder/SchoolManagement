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
}
