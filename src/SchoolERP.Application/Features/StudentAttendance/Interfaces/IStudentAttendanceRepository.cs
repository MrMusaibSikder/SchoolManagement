using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;
using System.Linq.Expressions;

namespace SchoolERP.Application.Features.StudentAttendance.Interfaces;

/// <summary>
/// Repository contract for <see cref="StudentAttendance"/> entities.
/// Extends the generic repository with a StudentAttendance-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IStudentAttendanceRepository : IGenericRepository<SchoolERP.Domain.Entities.StudentAttendance>
{
    /// <summary>
    /// Get attendance of a specific student for a specific date.
    /// Used for duplicate checking and update operation.
    /// </summary>
    Task<SchoolERP.Domain.Entities.StudentAttendance?>
        GetByStudentAndDateAsync(
        int studentId,
        DateTime attendanceDate,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Get all attendance records of a class and section for a specific date.
    /// Used when teacher opens attendance edit screen.
    /// </summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.StudentAttendance>> 
        GetByClassSectionDateAsync(
        int classId,
        int sectionId,
        DateTime attendanceDate,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Get student attendance history.
    /// Used for student profile and reports.
    /// </summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.StudentAttendance>> 
        GetStudentHistoryAsync(
        int studentId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Get attendance summary count.
    /// Used for dashboard and reports.
    /// </summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.StudentAttendance>> 
        GetAttendanceByDateAsync(
        DateTime attendanceDate,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchoolERP.Domain.Entities.StudentAttendance>>
        GetByStudentsAndDateAsync(
      IEnumerable<int> studentIds,
      DateTime attendanceDate,
      CancellationToken cancellationToken = default);
    /// <summary>
    /// Gets attendance records between two dates.
    /// Used for dashboard charts and reports.
    /// </summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.StudentAttendance>> GetAttendanceBetweenDatesAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets monthly attendance of a student.
    /// </summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.StudentAttendance>> GetMonthlyAttendanceAsync(
        int studentId,
        int month,
        int year,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets attendance of a class & section between two dates.
    /// </summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.StudentAttendance>> GetClassAttendanceAsync(
        int classId,
        int sectionId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether attendance already exists for a class & section.
    /// </summary>
    Task<bool> AttendanceExistsAsync(
        int classId,
        int sectionId,
        DateTime attendanceDate,
        CancellationToken cancellationToken = default);

    
}



