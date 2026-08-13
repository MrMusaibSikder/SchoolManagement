using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.FinalResult.DTOs;

namespace SchoolERP.Application.Features.FinalResult.Interfaces;

/// <summary>
/// Business/service contract for weighted final result calculation, ranking
/// and publishing. Combines every <see cref="SchoolERP.Domain.Entities.ExamResult"/>
/// for a student across an academic year, using the active
/// <see cref="SchoolERP.Domain.Entities.ExamWeightSetup"/>, into one
/// <see cref="SchoolERP.Domain.Entities.FinalResult"/> per student.
/// </summary>
public interface IFinalResultService
{
    /// <summary>
    /// (Re)calculates the weighted final result for every student who has
    /// marks in at least one weighted exam, using the academic year's active
    /// weight setup, then recomputes Merit/Class/Section positions. Idempotent
    /// — safe to re-run after correcting exam results (as long as not yet
    /// published).
    /// </summary>
    Task<IReadOnlyList<FinalResultDto>> CalculateAsync(int academicYearId, CancellationToken cancellationToken = default);

    /// <summary>Publishes every calculated final result for an academic year.</summary>
    Task<IReadOnlyList<FinalResultDto>> PublishAsync(int academicYearId, CancellationToken cancellationToken = default);

    /// <summary>Admin-only: unpublishes an academic year's final results so corrections can be made.</summary>
    Task UnpublishAsync(int academicYearId, CancellationToken cancellationToken = default);

    /// <summary>Gets a single student's final result (with subject breakdown) for an academic year.</summary>
    Task<FinalResultDto> GetStudentFinalResultAsync(int studentId, int academicYearId, CancellationToken cancellationToken = default);

    /// <summary>Gets every final result for an academic year, optionally restricted to a class.</summary>
    Task<IReadOnlyList<FinalResultDto>> GetByAcademicYearAsync(int academicYearId, int? classId, CancellationToken cancellationToken = default);

    /// <summary>Gets the ranked final-result merit list for a class within an academic year.</summary>
    Task<IReadOnlyList<MeritEntryDto>> GetClassMeritListAsync(int academicYearId, int classId, CancellationToken cancellationToken = default);

    /// <summary>Gets the ranked final-result merit list for a section within an academic year.</summary>
    Task<IReadOnlyList<MeritEntryDto>> GetSectionMeritListAsync(int academicYearId, int sectionId, CancellationToken cancellationToken = default);

    /// <summary>Sets the teacher and/or principal remark shown on a student's transcript for the year. Pass null to leave a remark unchanged.</summary>
    Task<FinalResultDto> SetRemarksAsync(int studentId, int academicYearId, string? teacherRemarks, string? principalRemarks, CancellationToken cancellationToken = default);
}
