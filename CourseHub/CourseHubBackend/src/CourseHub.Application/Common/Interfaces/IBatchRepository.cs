namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Minimal, read-only slice needed by Phase 11's public stats endpoint.
/// Phase 12 will extend this with the admin Batches CRUD.
/// </summary>
public interface IBatchRepository
{
    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);
}
