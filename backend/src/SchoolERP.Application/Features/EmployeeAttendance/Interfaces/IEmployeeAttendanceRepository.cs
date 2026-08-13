using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.EmployeeAttendance.Interfaces;

/// <summary>
/// Repository contract for <see cref="EmployeeAttendance"/> entities.
/// Extends the generic repository with an EmployeeAttendance-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IEmployeeAttendanceRepository : IGenericRepository<SchoolERP.Domain.Entities.EmployeeAttendance>
{
    /// <summary>
    /// Get attendance of a specific employee for a specific date.
    /// Used for duplicate checking and update operation.
    /// </summary>
    Task<SchoolERP.Domain.Entities.EmployeeAttendance?> GetByEmployeeAndDateAsync(
        int employeeId,
        DateTime attendanceDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get attendance of multiple employees for a specific date.
    /// Used by the bulk attendance workflow to detect existing records to update
    /// versus new records to create.
    /// </summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.EmployeeAttendance>> GetByEmployeesAndDateAsync(
        IEnumerable<int> employeeIds,
        DateTime attendanceDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get every employee's attendance for a specific date.
    /// Used for dashboard and daily-staff-attendance reports.
    /// </summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.EmployeeAttendance>> GetAttendanceByDateAsync(
        DateTime attendanceDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get attendance history of a specific employee, optionally bounded by a date range.
    /// </summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.EmployeeAttendance>> GetEmployeeHistoryAsync(
        int employeeId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets attendance records between two dates (all employees).
    /// Used for dashboard charts and reports.
    /// </summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.EmployeeAttendance>> GetAttendanceBetweenDatesAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);
}
