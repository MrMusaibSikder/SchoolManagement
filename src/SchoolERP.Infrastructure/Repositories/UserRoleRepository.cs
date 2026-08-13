using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.UserRole.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for the <see cref="UserRole"/> join entity.
/// Works only with the entity; never returns DTOs.
/// </summary>
public class UserRoleRepository : IUserRoleRepository
{
    private readonly SchoolERPDbContext _context;
    private readonly DbSet<UserRole> _dbSet;

    public UserRoleRepository(SchoolERPDbContext context)
    {
        _context = context;
        _dbSet = context.Set<UserRole>();
    }

    public async Task<UserRole?> GetAsync(int userId, int roleId, CancellationToken cancellationToken = default)
        => await _dbSet
        .Include(x=>x.Role)//new
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId, cancellationToken);

    public async Task<IReadOnlyList<UserRole>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet
        .Include(x => x.Role) //new
        .AsNoTracking().ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(int userId, int roleId, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().AnyAsync(x => x.UserId == userId && x.RoleId == roleId, cancellationToken);

    public async Task<UserRole> AddAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public void Remove(UserRole entity)
    {
        _dbSet.Remove(entity);
    }
}
