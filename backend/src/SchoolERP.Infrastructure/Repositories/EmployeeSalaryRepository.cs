using SchoolERP.Application.Features.EmployeeSalary.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="EmployeeSalary"/> entities.
/// Works only with the <see cref="EmployeeSalary"/> entity; never returns DTOs.
/// </summary>
public class EmployeeSalaryRepository : GenericRepository<EmployeeSalary>, IEmployeeSalaryRepository
{
    public EmployeeSalaryRepository(SchoolERPDbContext context) : base(context)
    {
    }
}
