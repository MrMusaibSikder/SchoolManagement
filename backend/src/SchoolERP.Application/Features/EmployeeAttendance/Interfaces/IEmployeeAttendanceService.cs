using SchoolERP.Application.Features.EmployeeAttendance.DTOs;

namespace SchoolERP.Application.Features.EmployeeAttendance.Interfaces;

/// <summary>
/// Business/service contract for EmployeeAttendance records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface IEmployeeAttendanceService
{
    /// <summary>Retrieves every EmployeeAttendance record.</summary>
    Task<IReadOnlyList<EmployeeAttendanceDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single EmployeeAttendance record by id, or null if it does not exist.</summary>
    Task<EmployeeAttendanceDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new EmployeeAttendance record.</summary>
    Task<EmployeeAttendanceDto> CreateAsync(CreateEmployeeAttendanceDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates attendance records for multiple employees on a single
    /// date in one transaction. Used for daily staff attendance entry.
    /// </summary>
    Task BulkAttendanceAsync(BulkEmployeeAttendanceDto request, CancellationToken cancellationToken = default);

    /// <summary>Gets every employee's attendance for a specific date.</summary>
    Task<IReadOnlyList<EmployeeAttendanceDto>> GetByDateAsync(DateTime attendanceDate, CancellationToken cancellationToken = default);

    /// <summary>Gets attendance history of a specific employee, optionally bounded by a date range.</summary>
    Task<IReadOnlyList<EmployeeAttendanceDto>> GetEmployeeHistoryAsync(
        int employeeId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    /// <summary>Updates an existing EmployeeAttendance record.</summary>
    Task<EmployeeAttendanceDto> UpdateAsync(int id, UpdateEmployeeAttendanceDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing EmployeeAttendance record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
