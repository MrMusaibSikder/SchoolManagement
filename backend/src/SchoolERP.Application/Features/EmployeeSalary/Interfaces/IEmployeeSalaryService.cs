using SchoolERP.Application.Features.EmployeeSalary.DTOs;

namespace SchoolERP.Application.Features.EmployeeSalary.Interfaces;

/// <summary>
/// Business/service contract for EmployeeSalary records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface IEmployeeSalaryService
{
    /// <summary>Retrieves every EmployeeSalary record.</summary>
    Task<IReadOnlyList<EmployeeSalaryDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single EmployeeSalary record by id, or null if it does not exist.</summary>
    Task<EmployeeSalaryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new EmployeeSalary record.</summary>
    Task<EmployeeSalaryDto> CreateAsync(CreateEmployeeSalaryDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing EmployeeSalary record.</summary>
    Task<EmployeeSalaryDto> UpdateAsync(int id, UpdateEmployeeSalaryDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing EmployeeSalary record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
