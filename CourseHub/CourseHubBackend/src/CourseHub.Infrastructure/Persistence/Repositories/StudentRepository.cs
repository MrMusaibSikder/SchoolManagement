using CourseHub.Application.Common.Interfaces;
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
}
