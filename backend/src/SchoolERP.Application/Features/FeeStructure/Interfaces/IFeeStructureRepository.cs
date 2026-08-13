using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.FeeStructure.Interfaces;

/// <summary>
/// Repository contract for <see cref="FeeStructure"/> entities.
/// Extends the generic repository with FeeStructure-specific data access members.
/// Contains database operations only.
/// </summary>
public interface IFeeStructureRepository : IGenericRepository<SchoolERP.Domain.Entities.FeeStructure>
{
    /// <summary>
    /// Gets the applicable fee structure for a given class, optional section, and academic year.
    /// Most frequently used during invoice generation.
    /// </summary>
    Task<SchoolERP.Domain.Entities.FeeStructure?> GetApplicableStructureAsync(
        int schoolClassId,
        int? sectionId,
        int academicYearId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a fee structure with its items and fee types (full detail, no tracking).
    /// </summary>
    Task<SchoolERP.Domain.Entities.FeeStructure?> GetWithItemsAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a tracked fee structure with its items and fee types.
    /// Used in update flows; service layer should handle merge logic.
    /// </summary>
    Task<SchoolERP.Domain.Entities.FeeStructure?> GetWithItemsTrackedAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a filtered list of fee structures.
    /// </summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.FeeStructure>> GetListAsync(
        int? academicYearId,
        int? schoolClassId,
        bool? isActive,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all fee structure templates (master copies not tied to a specific class/section).
    /// </summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.FeeStructure>> GetTemplatesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a fee structure already exists for the given class, section, and academic year.
    /// Used in the service layer to prevent duplicate structures.
    /// </summary>
    Task<bool> ExistsForClassSectionYearAsync(
        int schoolClassId,
        int? sectionId,
        int academicYearId,
        int? excludeId = null,
        CancellationToken cancellationToken = default);
   
}
