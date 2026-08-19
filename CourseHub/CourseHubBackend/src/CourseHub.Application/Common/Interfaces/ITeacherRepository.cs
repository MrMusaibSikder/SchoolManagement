using CourseHub.Domain.Entities;

namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Minimal, read-only slice needed by Phase 11's public endpoints.
/// Phase 12 (Admin/Private API) will extend this same interface with
/// GetByIdAsync/AddAsync/Update/Delete for the admin Teachers CRUD —
/// kept small here on purpose rather than guessing at that shape early.
/// </summary>
public interface ITeacherRepository
{
    /// <summary>
    /// Active teachers who have opted their profile into public listing
    /// (Teacher.IsProfilePublic) — see PublicCatalogService for how this
    /// is projected down to a privacy-safe DTO (no phone/email).
    /// </summary>
    Task<IReadOnlyList<Teacher>> GetPublicListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Count of active teachers, regardless of public-profile opt-in —
    /// used for the aggregate institute stats endpoint, which exposes a
    /// number only, never identifying individual teacher data.
    /// </summary>
    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);
}
