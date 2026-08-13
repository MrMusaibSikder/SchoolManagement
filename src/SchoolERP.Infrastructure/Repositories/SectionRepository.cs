using SchoolERP.Application.Features.Section.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="Section"/> entities.
/// Works only with the <see cref="Section"/> entity; never returns DTOs.
/// </summary>
public class SectionRepository : GenericRepository<Section>, ISectionRepository
{
    public SectionRepository(SchoolERPDbContext context) : base(context)
    {
    }
}
