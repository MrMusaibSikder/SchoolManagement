using SchoolERP.Application.Features.Employee.DTOs;

namespace SchoolERP.Application.Features.Employee.Interfaces;

/// <summary>
/// Business/service contract for Employee records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface IEmployeeService
{
    /// <summary>Retrieves every Employee record.</summary>
    Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single Employee record by id, or null if it does not exist.</summary>
    Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new Employee record.</summary>
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing Employee record.</summary>
    Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing Employee record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
