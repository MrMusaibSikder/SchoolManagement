using SchoolERP.Application.Features.StudentAttendance.DTOs;

namespace SchoolERP.Application.Features.StudentAttendance.Interfaces;

/// <summary>
/// Business logic contract for Student Attendance.
/// Handles bulk attendance, attendance retrieval,
/// update workflow and reporting operations.
/// </summary>
public interface IStudentAttendanceService
{

    /// <summary>
    /// Gets attendance records based on class, section and date.
    /// Used by teacher attendance screen.
    /// </summary>
    Task<IReadOnlyList<StudentAttendanceDto>> GetByClassSectionDateAsync(
        int classId,
        int sectionId,
        DateTime attendanceDate,
        CancellationToken cancellationToken = default);



    /// <summary>
    /// Creates or updates attendance records for multiple students.
    /// Used when teacher submits class attendance.
    /// </summary>
    Task BulkAttendanceAsync(
        BulkStudentAttendanceDto request,
        CancellationToken cancellationToken = default);



    /// <summary>
    /// Gets attendance history of a specific student.
    /// </summary>
    Task<IReadOnlyList<StudentAttendanceDto>> GetStudentHistoryAsync(
        int studentId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);



    /// <summary>
    /// Gets all attendance records.
    /// Mainly used for admin/report purposes.
    /// </summary>
    Task<IReadOnlyList<StudentAttendanceDto>> GetAllAsync(
        CancellationToken cancellationToken = default);



    /// <summary>
    /// Gets a single attendance record.
    /// </summary>
    Task<StudentAttendanceDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);



    /// <summary>
    /// Creates a single attendance record.
    /// Mainly for admin/manual entry.
    /// </summary>
    Task<StudentAttendanceDto> CreateAsync(
        CreateStudentAttendanceDto request,
        CancellationToken cancellationToken = default);



    /// <summary>
    /// Updates existing attendance record.
    /// </summary>
    Task<StudentAttendanceDto> UpdateAsync(
        int id,
        UpdateStudentAttendanceDto request,
        CancellationToken cancellationToken = default);



    /// <summary>
    /// Deletes attendance record using soft delete.
    /// </summary>
    Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);

}
