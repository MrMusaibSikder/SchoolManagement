using CourseHub.Application.Common.Interfaces;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Persistence.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly CourseHubDbContext _dbContext;

    public PermissionRepository(CourseHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return _dbContext.Permissions.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return _dbContext.Permissions.AnyAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions
            .OrderBy(p => p.Resource)
            .ThenBy(p => p.Action)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        await _dbContext.Permissions.AddAsync(permission, cancellationToken);
    }
}
