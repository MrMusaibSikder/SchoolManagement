using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.EmployeeSalary.Interfaces;

/// <summary>
/// Repository contract for <see cref="EmployeeSalary"/> entities.
/// Extends the generic repository with an EmployeeSalary-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IEmployeeSalaryRepository : IGenericRepository<SchoolERP.Domain.Entities.EmployeeSalary>
{
}
