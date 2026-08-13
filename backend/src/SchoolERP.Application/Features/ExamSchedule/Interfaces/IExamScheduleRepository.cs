using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Features.ExamSchedule.Interfaces;

/// <summary>
/// Repository contract for <see cref="SchoolERP.Domain.Entities.ExamSchedule"/> entities.
/// Extends the generic repository with ExamSchedule-specific data access members.
/// Contains database operations only; every business rule (Exam-status gating,
/// duplicate handling, etc.) lives in <c>IExamScheduleService</c>.
/// </summary>
public interface IExamScheduleRepository : IGenericRepository<SchoolERP.Domain.Entities.ExamSchedule>
{
    /// <summary>Gets a single schedule with its Exam, SchoolClass and Subject eagerly loaded.</summary>
    Task<SchoolERP.Domain.Entities.ExamSchedule?> GetByIdWithDetailsAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every schedule with its Exam, SchoolClass and Subject eagerly loaded.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.ExamSchedule>> GetAllWithDetailsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Gets every schedule for a given exam, ordered by exam date.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.ExamSchedule>> GetSchedulesByExamAsync(
        int examId,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every schedule for a given class, optionally restricted to a single exam.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.ExamSchedule>> GetSchedulesByClassAsync(
        int classId,
        int? examId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets every schedule for subjects taught by a given teacher (via the
    /// Subject -&gt; SubjectTeacher assignment), optionally restricted to a
    /// single exam.
    /// </summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.ExamSchedule>> GetSchedulesByTeacherAsync(
        int teacherId,
        int? examId,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every schedule whose exam date falls within the given (inclusive) date range, optionally restricted to a class.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.ExamSchedule>> GetExamCalendarAsync(
        DateTime fromDate,
        DateTime toDate,
        int? classId,
        CancellationToken cancellationToken = default);

    /// <summary>Checks whether the same subject is already scheduled for the same exam + class.</summary>
    Task<bool> DuplicateScheduleExistsAsync(
        int examId,
        int classId,
        int subjectId,
        int? excludeId,
        CancellationToken cancellationToken = default);

    /// <summary>Checks whether the same exam + class already has a (different) subject scheduled on the given date.</summary>
    Task<bool> DateAlreadyScheduledAsync(
        int examId,
        int classId,
        DateTime examDate,
        int? excludeId,
        CancellationToken cancellationToken = default);
}
