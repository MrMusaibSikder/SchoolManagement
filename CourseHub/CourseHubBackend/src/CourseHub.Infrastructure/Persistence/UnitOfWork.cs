using CourseHub.Application.Common.Interfaces;
using CourseHub.Infrastructure.Persistence.Context;

namespace CourseHub.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly CourseHubDbContext _dbContext;

    public UnitOfWork(CourseHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
