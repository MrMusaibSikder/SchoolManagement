using CourseHub.Application.Common.Interfaces;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly CourseHubDbContext _dbContext;

    public RoleRepository(CourseHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return _dbContext.Roles.AnyAsync(r => r.Name == name, cancellationToken);
    }

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        await _dbContext.Roles.AddAsync(role, cancellationToken);
    }
}
