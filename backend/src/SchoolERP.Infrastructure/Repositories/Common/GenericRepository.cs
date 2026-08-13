using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Common;
using SchoolERP.Infrastructure.Persistence.Context;
using System.Linq.Expressions;

namespace SchoolERP.Infrastructure.Repositories.Common;

/// <summary>
/// EF Core implementation of <see cref="IGenericRepository{TEntity}"/>.
/// Contains database access only; no business logic and no DTO mapping.
/// </summary>
/// <typeparam name="TEntity">The domain entity type.</typeparam>
public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly SchoolERPDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public GenericRepository(SchoolERPDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);

    public virtual async Task<TEntity?> GetByIdTrackedAsync(int id, CancellationToken cancellationToken = default)
        => await DbSet
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await DbSet
            .AsNoTracking()
            .Where(e => !e.IsDeleted)
            .ToListAsync(cancellationToken);

    public virtual async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        => await DbSet
            .AsNoTracking()
            .AnyAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
    public virtual async Task<bool> AnyAsync(
    Expression<Func<TEntity, bool>> predicate,
    CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .AnyAsync(predicate, cancellationToken);
    }
    //new
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => !x.IsDeleted)
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => !x.IsDeleted)
            .Where(predicate)
            .CountAsync(cancellationToken);
    }
    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual void Update(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        DbSet.Attach(entity);
        Context.Entry(entity).State = EntityState.Modified;
    }

    public virtual void Delete(TEntity entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        DbSet.Attach(entity);
        Context.Entry(entity).State = EntityState.Modified;
    }

    public virtual void HardDelete(TEntity entity)
    {
        DbSet.Remove(entity);
    }
}
