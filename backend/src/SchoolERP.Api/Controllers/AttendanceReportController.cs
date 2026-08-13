using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.AttendanceReport.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Read-only attendance reporting and dashboard endpoints, backed by
    /// <see cref="IAttendanceReportService"/>.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceReportController : ControllerBase
    {
        private readonly IAttendanceReportService _attendanceReportService;

        public AttendanceReportController(
            IAttendanceReportService attendanceReportService)
        {
            _attendanceReportService = attendanceReportService;
        }

        /// <summary>
        /// Dashboard summary (Today).
        /// </summary>
        [HttpGet("dashboard")]
        [PermissionAuthorize(PermissionNames.AttendanceReportView)]
        public async Task<IActionResult> GetDashboardSummary(
            CancellationToken cancellationToken)
        {
            var result = await _attendanceReportService
                .GetDashboardSummaryAsync(cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Student attendance summary.
        /// </summary>
        [HttpGet("student/{studentId}")]
        [PermissionAuthorize(PermissionNames.AttendanceReportView)]
        public async Task<IActionResult> GetStudentSummary(
            int studentId,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var result = await _attendanceReportService
                .GetStudentSummaryAsync(
                    studentId,
                    fromDate,
                    toDate,
                    cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Student monthly attendance report.
        /// </summary>
        [HttpGet("student/{studentId}/monthly")]
        [PermissionAuthorize(PermissionNames.AttendanceReportView)]
        public async Task<IActionResult> GetMonthlyReport(
            int studentId,
            int month,
            int year,
            CancellationToken cancellationToken)
        {
            var result = await _attendanceReportService
                .GetMonthlyReportAsync(
                    studentId,
                    month,
                    year,
                    cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Class attendance summary.
        /// </summary>
        [HttpGet("class-summary")]
        [PermissionAuthorize(PermissionNames.AttendanceReportView)]
        public async Task<IActionResult> GetClassSummary(
            int classId,
            int sectionId,
            DateTime attendanceDate,
            CancellationToken cancellationToken)
        {
            var result = await _attendanceReportService
                .GetClassSummaryAsync(
                    classId,
                    sectionId,
                    attendanceDate,
                    cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Teacher dashboard.
        /// </summary>
        [HttpGet("teacher-dashboard")]
        [PermissionAuthorize(PermissionNames.AttendanceReportView)]
        public async Task<IActionResult> GetTeacherDashboard(
            int classId,
            int sectionId,
            DateTime attendanceDate,
            CancellationToken cancellationToken)
        {
            var result = await _attendanceReportService
                .GetTeacherDashboardAsync(
                    classId,
                    sectionId,
                    attendanceDate,
                    cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Admin dashboard.
        /// </summary>
        [HttpGet("admin-dashboard")]
        [PermissionAuthorize(PermissionNames.AttendanceReportView)]
        public async Task<IActionResult> GetAdminDashboard(
            DateTime fromDate,
            DateTime toDate,
            CancellationToken cancellationToken)
        {
            var result = await _attendanceReportService
                .GetAdminDashboardAsync(
                    fromDate,
                    toDate,
                    cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Attendance trend.
        /// </summary>
        [HttpGet("trend")]
        [PermissionAuthorize(PermissionNames.AttendanceReportView)]
        public async Task<IActionResult> GetAttendanceTrend(
            DateTime fromDate,
            DateTime toDate,
            CancellationToken cancellationToken)
        {
            var result = await _attendanceReportService
                .GetAttendanceTrendAsync(
                    fromDate,
                    toDate,
                    cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Student attendance percentage.
        /// </summary>
        [HttpGet("student/{studentId}/percentage")]
        [PermissionAuthorize(PermissionNames.AttendanceReportView)]
        public async Task<IActionResult> GetStudentAttendancePercentage(
            int studentId,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var result = await _attendanceReportService
                .GetAttendancePercentageAsync(
                    studentId,
                    fromDate,
                    toDate,
                    cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Class attendance percentage.
        /// </summary>
        [HttpGet("class-percentage")]
        [PermissionAuthorize(PermissionNames.AttendanceReportView)]
        public async Task<IActionResult> GetClassAttendancePercentage(
            int classId,
            int sectionId,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var result = await _attendanceReportService
                .GetClassAttendancePercentageAsync(
                    classId,
                    sectionId,
                    fromDate,
                    toDate,
                    cancellationToken);

            return Ok(result);
        }
    }
}
