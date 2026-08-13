using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Features.GradeSetup.Interfaces;

/// <summary>
/// Repository contract for <see cref="SchoolERP.Domain.Entities.GradeSetup"/>
/// entities. Extends the generic repository with data access members needed
/// for per-academic-year grade band lookups. Contains database operations only.
/// </summary>
public interface IGradeSetupRepository : IGenericRepository<SchoolERP.Domain.Entities.GradeSetup>
{
    /// <summary>Gets every active grade band for an academic year, ordered by DisplayOrder.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.GradeSetup>> GetActiveByAcademicYearAsync(
        int academicYearId,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every grade band (active and inactive) for an academic year, with AcademicYear eagerly loaded.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.GradeSetup>> GetByAcademicYearAsync(
        int academicYearId,
        CancellationToken cancellationToken = default);

    /// <summary>Checks whether another (non-deleted) grade band already uses this name within the academic year.</summary>
    Task<bool> NameExistsAsync(
        int academicYearId,
        string gradeName,
        int? excludeId,
        CancellationToken cancellationToken = default);
}
