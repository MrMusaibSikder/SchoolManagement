using CourseHub.Domain.Entities;

namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Phase 11 added the read-only aggregate-count slice (CountActiveAsync).
/// Phase 12 extends it here with the full set needed by the admin
/// Students CRUD. Students are never listed publicly (unlike
/// Teachers/Courses) — there is deliberately no GetPublicListAsync here,
/// only the admin-facing SearchAsync below, which requires authentication
/// and the students.view permission.
/// </summary>
public interface IStudentRepository
{
    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);

    Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Looks up a student profile by the underlying User's id — the
    /// entry point for self-service endpoints (e.g. "my enrollments"),
    /// which only ever know the caller's UserId (from the JWT), never
    /// their Student.Id.
    /// </summary>
    Task<Student?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Student.UserId has a unique DB index — one student profile per
    /// user (see StudentConfiguration).
    /// </summary>
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// UserIds of every existing student profile — used to exclude
    /// already-promoted users from the "eligible users" dropdown (see
    /// IStudentService.GetEligibleUsersAsync).
    /// </summary>
    Task<IReadOnlyList<Guid>> GetAllUserIdsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Student.StudentId (the enrollment/roll id, not the DB primary key)
    /// has a unique DB index. <paramref name="excludingId"/> lets an
    /// Update check "does any *other* student already use this id"
    /// without the student's own unchanged id tripping a false positive.
    /// </summary>
    Task<bool> ExistsByStudentIdAsync(string studentId, Guid? excludingId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Admin student listing: optional name/student-id/email search,
    /// paged. Returns every student regardless of IsActive/IsProfilePublic
    /// — this is the admin screen, not the (nonexistent) public one.
    /// </summary>
    Task<(IReadOnlyList<Student> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(Student student, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Student>> GetByIdsAsync(
    IReadOnlyList<Guid> ids,
    CancellationToken cancellationToken = default);
}
