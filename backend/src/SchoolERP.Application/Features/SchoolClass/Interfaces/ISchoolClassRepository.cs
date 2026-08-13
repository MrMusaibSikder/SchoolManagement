using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.SchoolClass.Interfaces;

/// <summary>
/// Repository contract for <see cref="SchoolClass"/> entities.
/// Extends the generic repository with a SchoolClass-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface ISchoolClassRepository : IGenericRepository<SchoolERP.Domain.Entities.SchoolClass>
{
}
