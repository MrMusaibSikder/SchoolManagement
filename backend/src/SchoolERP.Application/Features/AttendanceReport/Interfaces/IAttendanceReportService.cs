using SchoolERP.Application.Features.AttendanceReport.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.AttendanceReport.Interfaces
{
    public interface IAttendanceReportService
    {
        /// <summary>
        /// Gets today's attendance summary for the admin dashboard.
        /// </summary>
        Task<DashboardAttendanceDto> GetDashboardSummaryAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets attendance summary of a specific student.
        /// </summary>
        Task<StudentAttendanceSummaryDto> GetStudentSummaryAsync(
            int studentId,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets monthly attendance report of a student.
        /// </summary>
        Task<MonthlyAttendanceReportDto> GetMonthlyReportAsync(
            int studentId,
            int month,
            int year,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets attendance summary of a class and section.
        /// </summary>
        Task<ClassAttendanceSummaryDto> GetClassSummaryAsync(
            int classId,
            int sectionId,
            DateTime attendanceDate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets teacher dashboard attendance summary.
        /// </summary>
        Task<TeacherDashboardAttendanceDto> GetTeacherDashboardAsync(
            int classId,
            int sectionId,
            DateTime attendanceDate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets admin dashboard attendance summary.
        /// </summary>
        Task<AdminDashboardAttendanceDto> GetAdminDashboardAsync(
            DateTime fromDate,
            DateTime toDate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets attendance trend report between two dates.
        /// </summary>
        Task<IReadOnlyList<AttendanceTrendDto>> GetAttendanceTrendAsync(
            DateTime fromDate,
            DateTime toDate,
            CancellationToken cancellationToken = default);



        /// <summary>
        /// Gets attendance percentage of a student.
        /// </summary>
        Task<double> GetAttendancePercentageAsync(
            int studentId,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets attendance percentage of a class.
        /// </summary>
        Task<double> GetClassAttendancePercentageAsync(
            int classId,
            int sectionId,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default);
    }
}