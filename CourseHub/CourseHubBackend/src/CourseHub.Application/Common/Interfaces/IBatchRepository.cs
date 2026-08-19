using CourseHub.Domain.Entities;

namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Phase 11 added the read-only aggregate-count slice (CountActiveAsync).
/// Phase 12 extends it here with the full set needed by the admin
/// Batches CRUD.
/// </summary>
public interface IBatchRepository
{
    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);

    Task<Batch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Batch.Code has a unique DB index (see BatchConfiguration).
    /// <paramref name="excludingId"/> lets an Update check "does any
    /// *other* batch already use this code" without the batch's own
    /// unchanged code tripping a false positive.
    /// </summary>
    Task<bool> ExistsByCodeAsync(string code, Guid? excludingId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Admin batch listing: optional name/code search, optionally scoped
    /// to a single course (e.g. "show me all batches of this course"),
    /// paged. Returns every batch regardless of IsActive.
    /// </summary>
    Task<(IReadOnlyList<Batch> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        Guid? courseId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(Batch batch, CancellationToken cancellationToken = default);
}
