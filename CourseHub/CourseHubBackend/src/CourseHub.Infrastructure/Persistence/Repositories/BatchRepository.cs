using CourseHub.Application.Common.Interfaces;
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
}
