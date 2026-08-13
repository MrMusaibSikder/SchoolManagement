using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.Designation.Interfaces;

/// <summary>
/// Repository contract for <see cref="Designation"/> entities.
/// Extends the generic repository with a Designation-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IDesignationRepository : IGenericRepository<SchoolERP.Domain.Entities.Designation>
{
}
