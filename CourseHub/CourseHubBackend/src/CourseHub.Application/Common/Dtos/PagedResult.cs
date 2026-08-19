namespace CourseHub.Application.Common.Dtos;

/// <summary>
/// Generic paging envelope. Introduced for Courses (Phase 12) but written
/// to be reused as-is by every other admin list endpoint coming next
/// (Teachers, Students, Batches, Enrollments) — avoid re-inventing a
/// paging shape per feature.
/// </summary>
public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
