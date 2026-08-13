using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Features.ExamWeightSetup.Interfaces;

/// <summary>
/// Repository contract for <see cref="SchoolERP.Domain.Entities.ExamWeightSetup"/>
/// entities. Extends the generic repository with data access members needed
/// for versioning/history and activation lookups. Contains database
/// operations only.
/// </summary>
public interface IExamWeightSetupRepository : IGenericRepository<SchoolERP.Domain.Entities.ExamWeightSetup>
{
    /// <summary>Gets a single setup with its Items (+Exam) and AcademicYear eagerly loaded.</summary>
    Task<SchoolERP.Domain.Entities.ExamWeightSetup?> GetByIdWithItemsAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every setup with its Items and AcademicYear eagerly loaded.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.ExamWeightSetup>> GetAllWithItemsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Gets every setup (active and inactive — version history) for a given academic year.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.ExamWeightSetup>> GetByAcademicYearAsync(
        int academicYearId,
        CancellationToken cancellationToken = default);

    /// <summary>Gets the currently active setup for a given academic year, with Items eagerly loaded, or null if none is active.</summary>
    Task<SchoolERP.Domain.Entities.ExamWeightSetup?> GetActiveByAcademicYearAsync(
        int academicYearId,
        CancellationToken cancellationToken = default);
}
