using SchoolERP.Application.Features.Result.DTOs;

namespace SchoolERP.Application.Features.Result.Interfaces;

/// <summary>
/// Business/service contract for Result (mark entry) records. Services return
/// DTOs only and encapsulate every business rule for this feature: teacher
/// assignment enforcement, exam-status gating, duplicate prevention, mark
/// range validation, draft/submit workflow, and lock/unlock.
/// </summary>
public interface IResultService
{
    /// <summary>Retrieves every Result record (enriched with names).</summary>
    Task<IReadOnlyList<ResultDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single Result record by id, or null if it does not exist.</summary>
    Task<ResultDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves every mark entry for a given exam schedule (one subject, one class).</summary>
    Task<IReadOnlyList<ResultDto>> GetByExamScheduleAsync(int examScheduleId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves every mark entry for a given student across every subject of one exam.</summary>
    Task<IReadOnlyList<ResultDto>> GetByStudentAndExamAsync(int studentId, int examId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a single mark entry as Draft. The exam must be Published, the
    /// teacher must be assigned to the schedule's subject, and no entry may
    /// already exist for this student + schedule.
    /// </summary>
    Task<ResultDto> CreateAsync(CreateResultDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing, unlocked mark entry. The teacher must be assigned to the schedule's subject.</summary>
    Task<ResultDto> UpdateAsync(int id, UpdateResultDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates marks for an entire class in one call (upsert per
    /// student). Used for both Bulk Marks Entry and Bulk Update.
    /// </summary>
    Task<IReadOnlyList<ResultDto>> BulkEntryAsync(BulkMarkEntryDto request, CancellationToken cancellationToken = default);

    /// <summary>Finalizes every Draft mark entry for an exam schedule, moving them to Submitted so they are ready for result calculation.</summary>
    Task<IReadOnlyList<ResultDto>> SubmitAsync(int examScheduleId, int teacherId, CancellationToken cancellationToken = default);

    /// <summary>Locks every mark entry for an exam schedule, preventing further edits (normally triggered by publishing the exam result).</summary>
    Task LockByExamScheduleAsync(int examScheduleId, CancellationToken cancellationToken = default);

    /// <summary>Admin-only: unlocks every mark entry for an exam schedule so corrections can be made.</summary>
    Task UnlockByExamScheduleAsync(int examScheduleId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing, unlocked mark entry.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
