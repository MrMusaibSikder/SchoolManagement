using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.Result.Interfaces;

/// <summary>
/// Repository contract for <see cref="Result"/> entities.
/// Extends the generic repository with a Result-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IResultRepository : IGenericRepository<SchoolERP.Domain.Entities.Result>
{
    /// <summary>Gets a single mark entry with Student/ExamSchedule (+Subject/Exam) eagerly loaded.</summary>
    Task<SchoolERP.Domain.Entities.Result?> GetByIdWithDetailsAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>Gets the mark entry for one student on one exam schedule, tracked (for update), or null.</summary>
    Task<SchoolERP.Domain.Entities.Result?> GetByStudentAndScheduleAsync(
        int studentId,
        int examScheduleId,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every mark entry for a given exam schedule (one subject, one class), with Student eagerly loaded.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Result>> GetByExamScheduleAsync(
        int examScheduleId,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every mark entry for a given student across every subject of one exam, with ExamSchedule (+Subject) eagerly loaded.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Result>> GetByStudentAndExamAsync(
        int studentId,
        int examId,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every mark entry for every student of a class across every subject of one exam. Used by result calculation and tabulation.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Result>> GetByClassAndExamAsync(
        int classId,
        int examId,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every mark entry for a student across every exam they have ever sat. Used by the Transcript module to avoid querying per exam.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Result>> GetByStudentAsync(
        int studentId,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every mark entry across every class/subject of one exam. Used by exam-wide result calculation.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Result>> GetByExamAsync(
        int examId,
        CancellationToken cancellationToken = default);

    /// <summary>Counts how many students of a class have at least one Submitted mark entry for an exam schedule.</summary>
    Task<int> CountSubmittedByScheduleAsync(
        int examScheduleId,
        CancellationToken cancellationToken = default);
}
