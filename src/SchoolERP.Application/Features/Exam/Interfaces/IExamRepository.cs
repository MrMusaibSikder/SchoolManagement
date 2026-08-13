using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Exam.Interfaces;

/// <summary>
/// Repository contract for <see cref="SchoolERP.Domain.Entities.Exam"/> entities.
/// Extends the generic repository with Exam-specific data access members.
/// Contains database operations only; every business rule (lifecycle
/// transitions, dashboard/report composition, etc.) lives in <c>IExamService</c>.
/// </summary>
public interface IExamRepository : IGenericRepository<SchoolERP.Domain.Entities.Exam>
{
    /// <summary>Gets a single exam with its ExamType and AcademicYear eagerly loaded, for display purposes.</summary>
    Task<SchoolERP.Domain.Entities.Exam?> GetByIdWithDetailsAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every exam with its ExamType and AcademicYear eagerly loaded, for display purposes.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Exam>> GetAllWithDetailsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single exam with ExamType, AcademicYear and every ExamSchedule
    /// (including each schedule's Subject and SchoolClass) eagerly loaded.
    /// </summary>
    Task<SchoolERP.Domain.Entities.Exam?> GetExamWithSchedulesAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single exam with everything needed to compute scheduling
    /// statistics (ExamType, AcademicYear, and every ExamSchedule with Subject
    /// and SchoolClass) eagerly loaded.
    /// </summary>
    Task<SchoolERP.Domain.Entities.Exam?> GetExamWithStatisticsAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets published exams that still have at least one schedule dated on/after
    /// <paramref name="asOfDate"/>, ordered by their earliest upcoming schedule
    /// date, limited to <paramref name="count"/> results.
    /// </summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Exam>> GetUpcomingExamsAsync(
        DateTime asOfDate,
        int count,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every exam whose status is <see cref="ExamStatus.Completed"/>.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Exam>> GetCompletedExamsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Gets the most recently created exams, with ExamType/AcademicYear eagerly loaded.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Exam>> GetRecentExamsAsync(
        int count,
        CancellationToken cancellationToken = default);

    /// <summary>Counts exams, optionally restricted to a single lifecycle status.</summary>
    Task<int> CountByStatusAsync(
        ExamStatus? status,
        CancellationToken cancellationToken = default);

    /// <summary>Gets just the lifecycle status of an exam, without loading the full entity. Null if the exam does not exist.</summary>
    Task<ExamStatus?> GetExamStatusAsync(
        int examId,
        CancellationToken cancellationToken = default);

    /// <summary>Checks whether a (non-deleted) exam with the given id exists.</summary>
    Task<bool> ExamExistsAsync(
        int examId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether another (non-deleted) exam already exists with the same
    /// AcademicYear + ExamType + Name combination (case-insensitive name match).
    /// </summary>
    Task<bool> DuplicateExamExistsAsync(
        string name,
        int academicYearId,
        int examTypeId,
        int? excludeId,
        CancellationToken cancellationToken = default);
}
