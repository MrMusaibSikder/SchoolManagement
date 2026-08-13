using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.Section.Interfaces;

/// <summary>
/// Repository contract for <see cref="Section"/> entities.
/// Extends the generic repository with a Section-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface ISectionRepository : IGenericRepository<SchoolERP.Domain.Entities.Section>
{
}
