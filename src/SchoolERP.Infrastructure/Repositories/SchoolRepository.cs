using SchoolERP.Application.Features.School.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="School"/> entities.
/// Works only with the <see cref="School"/> entity; never returns DTOs.
/// </summary>
public class SchoolRepository : GenericRepository<School>, ISchoolRepository
{
    public SchoolRepository(SchoolERPDbContext context) : base(context)
    {
    }
}
