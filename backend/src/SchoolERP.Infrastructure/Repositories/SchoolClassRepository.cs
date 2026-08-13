using SchoolERP.Application.Features.SchoolClass.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="SchoolClass"/> entities.
/// Works only with the <see cref="SchoolClass"/> entity; never returns DTOs.
/// </summary>
public class SchoolClassRepository : GenericRepository<SchoolClass>, ISchoolClassRepository
{
    public SchoolClassRepository(SchoolERPDbContext context) : base(context)
    {
    }
}
