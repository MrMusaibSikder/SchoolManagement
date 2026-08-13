using SchoolERP.Domain.Common;
using System.Linq.Expressions;

namespace SchoolERP.Application.Common.Interfaces;

/// <summary>
/// Generic, entity-only data access contract shared by every feature repository.
/// Repositories work exclusively with <see cref="BaseEntity"/> derived types and
/// never expose or accept DTOs.
/// </summary>
/// <typeparam name="TEntity">The domain entity type.</typeparam>
public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    /// <summary>Gets a single, non-deleted entity by its primary key (read-only, no tracking).</summary>
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Gets a single, non-deleted entity by its primary key with change tracking enabled.</summary>
    Task<TEntity?> GetByIdTrackedAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Gets every non-deleted entity (read-only, no tracking).</summary>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Checks whether a non-deleted entity with the given id exists.</summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Adds a new entity to the change tracker.</summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>Marks an existing entity as modified.</summary>
    void Update(TEntity entity);

    /// <summary>Soft-deletes an entity (sets IsDeleted / DeletedAt).</summary>
    void Delete(TEntity entity);

    /// <summary>Physically removes an entity from the database. Use with caution.</summary>
    void HardDelete(TEntity entity);
    // new repositor add by Musaib Sikder
    Task<bool> AnyAsync(
    Expression<Func<TEntity, bool>> predicate,
    CancellationToken cancellationToken = default);
    //new 
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}
