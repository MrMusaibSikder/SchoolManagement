using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.School.Interfaces;

/// <summary>
/// Repository contract for <see cref="School"/> entities.
/// Extends the generic repository with a School-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface ISchoolRepository : IGenericRepository<SchoolERP.Domain.Entities.School>
{
}
