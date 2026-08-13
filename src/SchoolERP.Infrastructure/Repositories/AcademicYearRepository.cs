using SchoolERP.Application.Features.AcademicYear.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="AcademicYear"/> entities.
/// Works only with the <see cref="AcademicYear"/> entity; never returns DTOs.
/// </summary>
public class AcademicYearRepository : GenericRepository<AcademicYear>, IAcademicYearRepository
{
    public AcademicYearRepository(SchoolERPDbContext context) : base(context)
    {
    }
}
