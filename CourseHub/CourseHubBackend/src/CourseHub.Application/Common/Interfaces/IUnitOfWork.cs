namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Persists changes made through repositories in a single transaction.
/// Application depends on this abstraction only; Infrastructure implements
/// it by wrapping the EF Core DbContext's SaveChangesAsync.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
