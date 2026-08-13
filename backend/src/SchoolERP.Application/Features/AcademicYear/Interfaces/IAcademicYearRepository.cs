using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.AcademicYear.Interfaces;

/// <summary>
/// Repository contract for <see cref="AcademicYear"/> entities.
/// Extends the generic repository with an AcademicYear-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IAcademicYearRepository : IGenericRepository<SchoolERP.Domain.Entities.AcademicYear>
{
}
