using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.EmployeeAttendance.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="EmployeeAttendance"/> entities.
/// Works only with the <see cref="EmployeeAttendance"/> entity; never returns DTOs.
/// </summary>
public class EmployeeAttendanceRepository : GenericRepository<EmployeeAttendance>, IEmployeeAttendanceRepository
{
    public EmployeeAttendanceRepository(SchoolERPDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<EmployeeAttendance?> GetByEmployeeAndDateAsync(
        int employeeId,
        DateTime attendanceDate,
        CancellationToken cancellationToken = default)
    {
        var date = attendanceDate.Date;

        return await DbSet
            .FirstOrDefaultAsync(x =>
                !x.IsDeleted &&
                x.EmployeeId == employeeId &&
                x.AttendanceDate.Date == date,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EmployeeAttendance>> GetByEmployeesAndDateAsync(
        IEnumerable<int> employeeIds,
        DateTime attendanceDate,
        CancellationToken cancellationToken = default)
    {
        var date = attendanceDate.Date;

        return await DbSet
            .Where(x =>
                !x.IsDeleted &&
                x.AttendanceDate.Date == date &&
                employeeIds.Contains(x.EmployeeId))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EmployeeAttendance>> GetAttendanceByDateAsync(
        DateTime attendanceDate,
        CancellationToken cancellationToken = default)
    {
        var date = attendanceDate.Date;

        return await DbSet
            .AsNoTracking()
            .Include(x => x.Employee)
            .Where(x =>
                !x.IsDeleted &&
                x.AttendanceDate.Date == date)
            .OrderBy(x => x.Employee!.FullName)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EmployeeAttendance>> GetEmployeeHistoryAsync(
        int employeeId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EmployeeAttendance> query = DbSet
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.EmployeeId == employeeId);

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.AttendanceDate >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x => x.AttendanceDate <= toDate.Value.Date);
        }

        return await query
            .OrderByDescending(x => x.AttendanceDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EmployeeAttendance>> GetAttendanceBetweenDatesAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var from = fromDate.Date;
        var to = toDate.Date;

        return await DbSet
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.AttendanceDate >= from &&
                x.AttendanceDate <= to)
            .OrderBy(x => x.AttendanceDate)
            .ToListAsync(cancellationToken);
    }
}
