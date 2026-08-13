using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.Student.Interfaces;

/// <summary>
/// Repository contract for <see cref="Student"/> entities.
/// Extends the generic repository with a Student-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IStudentRepository : IGenericRepository<SchoolERP.Domain.Entities.Student>
{
    // new Add By Musaib Sikder
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Student>> GetActiveByClassSectionAsync(
        int schoolClassId, int? sectionId, CancellationToken cancellationToken = default);
    //new Add By Musaib Sikder
    Task<SchoolERP.Domain.Entities.Student?> GetByIdWithGuardiansAsync(
        int id,
        CancellationToken cancellationToken = default);
    //new Add By Musaib Sikder
    Task<SchoolERP.Domain.Entities.Student?> GetByIdWithGuardiansTrackedAsync(
        int id,
        CancellationToken cancellationToken = default);
    //A Method  For Get All data with gardian Crated by Mudsaib Sikder
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Student>> GetAllWithGuardiansAsync(
       CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Student>> GetByClassAndSectionAsync(
    int classId,
    int sectionId,
    CancellationToken cancellationToken = default);
    /// <summary>Gets all active students enrolled in the given academic year.</summary>
   
}
