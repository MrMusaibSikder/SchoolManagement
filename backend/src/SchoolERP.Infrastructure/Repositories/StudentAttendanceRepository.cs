using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.StudentAttendance.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="StudentAttendance"/> entities.
/// Works only with the <see cref="StudentAttendance"/> entity; never returns DTOs.
/// </summary>
public class StudentAttendanceRepository : GenericRepository<StudentAttendance>, IStudentAttendanceRepository
{
    public StudentAttendanceRepository(SchoolERPDbContext context) : base(context)
    {
    }
    
    
    ///<inheritdoc/>
    public async Task<StudentAttendance?> GetByStudentAndDateAsync(
        int studentId,
        DateTime attendanceDate,
        CancellationToken cancellationToken = default)
    {
        var date = attendanceDate.Date;


        return await DbSet
            .FirstOrDefaultAsync(x =>
                !x.IsDeleted &&
                x.StudentId == studentId &&
                x.AttendanceDate.Date == date,
                cancellationToken);
    }
    ///<inheritdoc/>
    public async Task<IReadOnlyList<StudentAttendance>>
        GetByClassSectionDateAsync(
            int classId,
            int sectionId,
            DateTime attendanceDate,
            CancellationToken cancellationToken = default)
    {

        var date = attendanceDate.Date;


        return await DbSet
            .AsNoTracking()
            .Include(x => x.Student)
            .Where(x =>
                !x.IsDeleted &&
                x.AttendanceDate.Date == date &&
                x.Student != null &&
                x.Student.ClassId == classId &&
                x.Student.SectionId == sectionId)
            .OrderBy(x => x.Student!.RollNo)
            .ToListAsync(cancellationToken);

    }
    ///<inheritdoc/>
    public async Task<IReadOnlyList<StudentAttendance>>
        GetStudentHistoryAsync(
            int studentId,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default)
    {


        IQueryable<StudentAttendance> query = DbSet
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.StudentId == studentId);



        if (fromDate.HasValue)
        {
            query = query.Where(x =>
                x.AttendanceDate >= fromDate.Value.Date);
        }



        if (toDate.HasValue)
        {
            query = query.Where(x =>
                x.AttendanceDate <= toDate.Value.Date);
        }



        return await query
            .OrderByDescending(x => x.AttendanceDate)
            .ToListAsync(cancellationToken);

    }
    ///<inheritdoc/>
    public async Task<IReadOnlyList<StudentAttendance>>
       GetAttendanceByDateAsync(
           DateTime attendanceDate,
           CancellationToken cancellationToken = default)
    {

        var date = attendanceDate.Date;


        return await DbSet
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.AttendanceDate.Date == date)
            .ToListAsync(cancellationToken);

    }
    ///<inheritdoc/>
    public async Task<IReadOnlyList<StudentAttendance>> GetByStudentsAndDateAsync(
    IEnumerable<int> studentIds,
    DateTime attendanceDate,
    CancellationToken cancellationToken = default)
    {
        var date = attendanceDate.Date;

        return await DbSet
            .Where(x =>
                !x.IsDeleted &&
                x.AttendanceDate.Date == date &&
                studentIds.Contains(x.StudentId))
            .ToListAsync(cancellationToken);
    }
    ///<inheritdoc/>
    public async Task<IReadOnlyList<StudentAttendance>> GetAttendanceBetweenDatesAsync(
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
    ///<inheritdoc/>
    public async Task<IReadOnlyList<StudentAttendance>> GetMonthlyAttendanceAsync(
    int studentId,
    int month,
    int year,
    CancellationToken cancellationToken = default)
    {
        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1).AddDays(-1);

        return await DbSet
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.StudentId == studentId &&
                x.AttendanceDate >= from &&
                x.AttendanceDate <= to)
            .OrderBy(x => x.AttendanceDate)
            .ToListAsync(cancellationToken);
    }
    ///<inheritdoc/>
    public async Task<IReadOnlyList<StudentAttendance>> GetClassAttendanceAsync(
    int classId,
    int sectionId,
    DateTime fromDate,
    DateTime toDate,
    CancellationToken cancellationToken = default)
    {
        var from = fromDate.Date;
        var to = toDate.Date;

        return await DbSet
            .AsNoTracking()
            .Include(x => x.Student)
            .Where(x =>
                !x.IsDeleted &&
                x.Student != null &&
                x.Student.ClassId == classId &&
                x.Student.SectionId == sectionId &&
                x.AttendanceDate >= from &&
                x.AttendanceDate <= to)
            .OrderBy(x => x.AttendanceDate)
            .ThenBy(x => x.Student!.RollNo)
            .ToListAsync(cancellationToken);
    }
    ///<inheritdoc/>
    public async Task<bool> AttendanceExistsAsync(
    int classId,
    int sectionId,
    DateTime attendanceDate,
    CancellationToken cancellationToken = default)
    {
        var date = attendanceDate.Date;

        return await DbSet
            .AsNoTracking()
            .AnyAsync(x =>
                !x.IsDeleted &&
                x.AttendanceDate == date &&
                x.Student != null &&
                x.Student.ClassId == classId &&
                x.Student.SectionId == sectionId,
                cancellationToken);
    }
}
