using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Features.FinalResult.Interfaces;

/// <summary>
/// Repository contract for <see cref="SchoolERP.Domain.Entities.FinalResult"/>
/// entities. Extends the generic repository with FinalResult-specific data
/// access members. Contains database operations only; calculation, ranking
/// and publishing rules live in <c>IFinalResultService</c>.
/// </summary>
public interface IFinalResultRepository : IGenericRepository<SchoolERP.Domain.Entities.FinalResult>
{
    /// <summary>Gets a single final result with Student, AcademicYear and Details (+Subject) eagerly loaded, or null.</summary>
    Task<SchoolERP.Domain.Entities.FinalResult?> GetByIdWithDetailsAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>Gets a single final result with its Details collection tracked (not AsNoTracking), for replacing subject details during recalculation.</summary>
    Task<SchoolERP.Domain.Entities.FinalResult?> GetByIdTrackedWithDetailsAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>Gets one student's final result for an academic year, with Details eagerly loaded, or null.</summary>
    Task<SchoolERP.Domain.Entities.FinalResult?> GetByStudentAndYearAsync(
        int studentId,
        int academicYearId,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every final result for an academic year, with Student (+Class/Section) eagerly loaded.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.FinalResult>> GetByAcademicYearAsync(
        int academicYearId,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every final result for an academic year restricted to one class, with Student eagerly loaded.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.FinalResult>> GetByAcademicYearAndClassAsync(
        int academicYearId,
        int classId,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every final result for an academic year restricted to one section, with Student eagerly loaded.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.FinalResult>> GetByAcademicYearAndSectionAsync(
        int academicYearId,
        int sectionId,
        CancellationToken cancellationToken = default);
}
