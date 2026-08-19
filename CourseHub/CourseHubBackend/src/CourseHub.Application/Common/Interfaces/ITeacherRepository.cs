using CourseHub.Domain.Entities;

namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Phase 11 added the read-only public-catalog slice (GetPublicListAsync,
/// CountActiveAsync). Phase 12 extends it here with the full set needed
/// by the admin Teachers CRUD.
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

    Task<Teacher?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Teacher.UserId has a unique DB index — one teaching profile per
    /// user (see TeacherConfiguration). Used to reject creating a second
    /// profile for a user who already has one.
    /// </summary>
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Teacher.EmployeeId has a unique DB index. <paramref name="excludingId"/>
    /// lets an Update check "does any *other* teacher already use this
    /// employee id" without the teacher's own unchanged id tripping a
    /// false positive.
    /// </summary>
    Task<bool> ExistsByEmployeeIdAsync(string employeeId, Guid? excludingId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Admin teacher listing: optional name/employee-id/email search,
    /// paged. Returns every teacher regardless of IsActive/IsProfilePublic
    /// — unlike GetPublicListAsync, this is for the admin screen where
    /// inactive/private teachers must still be visible to manage.
    /// </summary>
    Task<(IReadOnlyList<Teacher> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(Teacher teacher, CancellationToken cancellationToken = default);
}
