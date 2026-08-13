using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.ExamResult.DTOs;

namespace SchoolERP.Application.Features.ExamResult.Interfaces;

/// <summary>
/// Business/service contract for exam result calculation, ranking,
/// publishing and reporting. Aggregates Result (mark entry) rows into one
/// <see cref="SchoolERP.Domain.Entities.ExamResult"/> per student per exam,
/// computes Class/Section/Merit positions, and exposes every report/dashboard
/// view built on top of that data.
/// </summary>
public interface IExamResultService
{
    /// <summary>
    /// (Re)calculates the aggregate exam result for every student who has at
    /// least one Submitted mark entry for the exam, then recomputes
    /// Merit/Class/Section positions across the whole exam. Idempotent — safe
    /// to re-run after correcting marks (as long as the result is not yet
    /// published).
    /// </summary>
    Task<IReadOnlyList<ExamResultDto>> CalculateAsync(int examId, CancellationToken cancellationToken = default);

    /// <summary>Publishes every calculated result for an exam, locking the underlying mark entries.</summary>
    Task<IReadOnlyList<ExamResultDto>> PublishAsync(int examId, CancellationToken cancellationToken = default);

    /// <summary>Admin-only: unpublishes an exam's results and unlocks the underlying mark entries so corrections can be made.</summary>
    Task UnpublishAsync(int examId, CancellationToken cancellationToken = default);

    /// <summary>Gets a single student's full result (summary + subject breakdown) for one exam.</summary>
    Task<StudentExamResultDto> GetStudentResultAsync(int studentId, int examId, CancellationToken cancellationToken = default);

    /// <summary>Gets every aggregate result for an exam, optionally restricted to a class.</summary>
    Task<IReadOnlyList<ExamResultDto>> GetByExamAsync(int examId, int? classId, CancellationToken cancellationToken = default);

    /// <summary>Gets the full subject-by-student tabulation sheet for a class within an exam.</summary>
    Task<TabulationSheetDto> GetTabulationSheetAsync(int examId, int classId, CancellationToken cancellationToken = default);

    /// <summary>Gets the ranked merit list for a class within an exam.</summary>
    Task<IReadOnlyList<MeritEntryDto>> GetClassMeritListAsync(int examId, int classId, CancellationToken cancellationToken = default);

    /// <summary>Gets the ranked merit list for a section within an exam.</summary>
    Task<IReadOnlyList<MeritEntryDto>> GetSectionMeritListAsync(int examId, int sectionId, CancellationToken cancellationToken = default);

    /// <summary>Gets every student who failed the exam, optionally restricted to a class.</summary>
    Task<IReadOnlyList<MeritEntryDto>> GetFailedStudentsAsync(int examId, int? classId, CancellationToken cancellationToken = default);

    /// <summary>Gets the top-performing students for the exam, optionally restricted to a class.</summary>
    Task<IReadOnlyList<MeritEntryDto>> GetTopStudentsAsync(int examId, int? classId, int count, CancellationToken cancellationToken = default);

    /// <summary>Gets highest/lowest/average marks and pass rate for every subject of an exam.</summary>
    Task<IReadOnlyList<SubjectStatisticsDto>> GetSubjectStatisticsAsync(int examId, CancellationToken cancellationToken = default);

    /// <summary>Gets the number (and percentage) of students achieving each grade in the exam.</summary>
    Task<IReadOnlyList<GradeDistributionItemDto>> GetGradeDistributionAsync(int examId, CancellationToken cancellationToken = default);

    /// <summary>Gets result-processing progress and outcome statistics for the exam dashboard.</summary>
    Task<ExamResultDashboardDto> GetDashboardAsync(int examId, CancellationToken cancellationToken = default);

    /// <summary>Sets the teacher and/or guardian-facing remark for a student's result in this exam. Pass null for a remark to leave it unchanged.</summary>
    Task<ExamResultDto> SetRemarksAsync(int studentId, int examId, string? teacherRemarks, string? guardianRemarks, CancellationToken cancellationToken = default);
}
