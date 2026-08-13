using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.RolePermission.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for the <see cref="RolePermission"/> join entity.
/// Works only with the entity; never returns DTOs.
/// </summary>
public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly SchoolERPDbContext _context;
    private readonly DbSet<RolePermission> _dbSet;

    public RolePermissionRepository(SchoolERPDbContext context)
    {
        _context = context;
        _dbSet = context.Set<RolePermission>();
    }

    public async Task<RolePermission?> GetAsync(int roleId, int permissionId, CancellationToken cancellationToken = default)
        => await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RoleId == roleId && x.PermissionId == permissionId, cancellationToken);

    public async Task<IReadOnlyList<RolePermission>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(int roleId, int permissionId, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().AnyAsync(x => x.RoleId == roleId && x.PermissionId == permissionId, cancellationToken);

    public async Task<RolePermission> AddAsync(RolePermission entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public void Remove(RolePermission entity)
    {
        _dbSet.Remove(entity);
    }
}
