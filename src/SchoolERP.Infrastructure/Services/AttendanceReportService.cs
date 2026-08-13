using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.AttendanceReport.DTOs;
using SchoolERP.Application.Features.AttendanceReport.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Services
{
    public class AttendanceReportService : IAttendanceReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <inheritdoc/>
        public async Task<StudentAttendanceSummaryDto> GetStudentSummaryAsync(
            int studentId,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default)
        {
            // Validate Student
            var student = await _unitOfWork.StudentRepository
                .GetByIdAsync(studentId, cancellationToken);

            if (student is null)
            {
                throw new NotFoundException(nameof(Student), studentId);
            }

            // Load Attendance
            var attendances = await _unitOfWork.StudentAttendanceRepository
     .GetStudentHistoryAsync(
         studentId,
         fromDate,
         toDate,
         cancellationToken);

            var summary = CalculateAttendanceSummary(attendances);

            return new StudentAttendanceSummaryDto
            {
                StudentId = studentId,
                TotalDays = summary.Total,
                Present = summary.Present,
                Absent = summary.Absent,
                Late = summary.Late,
                Leave = summary.Leave,
                Percentage = summary.Percentage
            };
        }

        /// <inheritdoc/>
        public async Task<MonthlyAttendanceReportDto> GetMonthlyReportAsync(
            int studentId,
            int month,
            int year,
            CancellationToken cancellationToken = default)
        {
            // Student Validation
            var student = await _unitOfWork.StudentRepository
                .GetByIdAsync(studentId, cancellationToken);

            if (student is null)
            {
                throw new NotFoundException(nameof(Student), studentId);
            }

            // Attendance
            var attendances = await _unitOfWork.StudentAttendanceRepository
                .GetMonthlyAttendanceAsync(
                    studentId,
                    month,
                    year,
                    cancellationToken);
            var summary = CalculateAttendanceSummary(attendances);

            return new MonthlyAttendanceReportDto
            {
                Month = month,
                Year = year,
                TotalSchoolDays = summary.Total,
                Present = summary.Present,
                Absent = summary.Absent,
                Late = summary.Late,
                Leave = summary.Leave,
                Percentage = summary.Percentage
            };
        }

       
        public async Task<DashboardAttendanceDto> GetDashboardSummaryAsync(
     CancellationToken cancellationToken = default)
        {
            var attendanceDate = DateTime.Today;

            var attendances = await _unitOfWork.StudentAttendanceRepository
                .GetAttendanceByDateAsync(
                    attendanceDate,
                    cancellationToken);

            var summary = CalculateAttendanceSummary(attendances);

            return new DashboardAttendanceDto
            {
                TotalStudents = summary.Total,
                PresentToday = summary.Present,
                AbsentToday = summary.Absent,
                LateToday = summary.Late,
                LeaveToday = summary.Leave,
                AttendancePercentage = summary.Percentage
            };
        }
        /// <inheritdoc/>
        public async Task<TeacherDashboardAttendanceDto> GetTeacherDashboardAsync(
            int classId,
            int sectionId,
            DateTime attendanceDate,
            CancellationToken cancellationToken = default)
        {
            var attendances = await _unitOfWork.StudentAttendanceRepository
                .GetByClassSectionDateAsync(
                    classId,
                    sectionId,
                    attendanceDate,
                    cancellationToken);

            var totalStudents = attendances
                .Select(x => x.StudentId)
                .Distinct()
                .Count();

            var summary = CalculateAttendanceSummary(attendances);

            return new TeacherDashboardAttendanceDto
            {
                ClassId = classId,
                SectionId = sectionId,
                TotalStudents = totalStudents,
                Present = summary.Present,
                Absent = summary.Absent,
                Late = summary.Late,
                Leave = summary.Leave,
                Percentage = summary.Percentage
            };
        }
        /// <inheritdoc/>
        public async Task<AdminDashboardAttendanceDto> GetAdminDashboardAsync(
            DateTime fromDate,
            DateTime toDate,
            CancellationToken cancellationToken = default)
        {
            var attendances = await _unitOfWork.StudentAttendanceRepository
                .GetAttendanceBetweenDatesAsync(
                    fromDate,
                    toDate,
                    cancellationToken);

            var totalStudents = attendances
                .Select(x => x.StudentId)
                .Distinct()
                .Count();

            var present = attendances.Count(x => x.Status == AttendanceStatus.Present);

            var absent = attendances.Count(x => x.Status == AttendanceStatus.Absent);

            var late = attendances.Count(x => x.Status == AttendanceStatus.Late);

            var leave = attendances.Count(x => x.Status == AttendanceStatus.Leave);

            var totalAttendance = attendances.Count;

            var percentage = totalAttendance == 0
                ? 0
                : Math.Round((double)present / totalAttendance * 100, 2);

            return new AdminDashboardAttendanceDto
            {
                TotalStudents = totalStudents,
                TotalPresent = present,
                TotalAbsent = absent,
                TotalLate = late,
                TotalLeave = leave,
                AttendancePercentage = percentage
            };
        }
        /// <inheritdoc/>
        public async Task<IReadOnlyList<AttendanceTrendDto>> GetAttendanceTrendAsync(
            DateTime fromDate,
            DateTime toDate,
            CancellationToken cancellationToken = default)
        {
            if (fromDate.Date > toDate.Date)
            {
                throw new InvalidOperationException("From date cannot be greater than To date.");
            }

            var attendances = await _unitOfWork.StudentAttendanceRepository
                .GetAttendanceBetweenDatesAsync(
                    fromDate,
                    toDate,
                    cancellationToken);

            var result = attendances
                .GroupBy(x => x.AttendanceDate.Date)
                .OrderBy(x => x.Key)
                .Select(g => new AttendanceTrendDto
                {
                    Date = g.Key,

                    Present = g.Count(x =>
                        x.Status == AttendanceStatus.Present),

                    Absent = g.Count(x =>
                        x.Status == AttendanceStatus.Absent),

                    Late = g.Count(x =>
                        x.Status == AttendanceStatus.Late),

                    Leave = g.Count(x =>
                        x.Status == AttendanceStatus.Leave)
                })
                .ToList();

            return result;
        }
        /// <inheritdoc/>
        public async Task<ClassAttendanceSummaryDto> GetClassSummaryAsync(
            int classId,
            int sectionId,
            DateTime attendanceDate,
            CancellationToken cancellationToken = default)
        {
            var attendances = await _unitOfWork.StudentAttendanceRepository
                .GetByClassSectionDateAsync(
                    classId,
                    sectionId,
                    attendanceDate,
                    cancellationToken);

            var totalStudents = attendances
                .Select(x => x.StudentId)
                .Distinct()
                .Count();

            var summary = CalculateAttendanceSummary(attendances);

            return new ClassAttendanceSummaryDto
            {
                ClassId = classId,
                SectionId = sectionId,
                TotalStudents = totalStudents,
                Present = summary.Present,
                Absent = summary.Absent,
                Late = summary.Late,
                Leave = summary.Leave,
                Percentage = summary.Percentage
            };
        }

        /// <inheritdoc/>
        public async Task<double> GetAttendancePercentageAsync(
            int studentId,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default)
        {
            var student = await _unitOfWork.StudentRepository
                .GetByIdAsync(studentId, cancellationToken);

            if (student is null)
            {
                throw new NotFoundException(nameof(Student), studentId);
            }

            var attendances = await _unitOfWork.StudentAttendanceRepository
                .GetStudentHistoryAsync(
                    studentId,
                    fromDate,
                    toDate,
                    cancellationToken);

            if (!attendances.Any())
            {
                return 0;
            }

            var present = attendances.Count(x => x.Status == AttendanceStatus.Present);

            return Math.Round(
                (double)present / attendances.Count * 100,
                2);
        }

        /// <inheritdoc/>
        public async Task<double> GetClassAttendancePercentageAsync(
            int classId,
            int sectionId,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default)
        {
            var attendances = await _unitOfWork.StudentAttendanceRepository
                .GetClassAttendanceAsync(
                    classId,
                    sectionId,
                    fromDate ?? DateTime.MinValue,
                    toDate ?? DateTime.MaxValue,
                    cancellationToken);

            if (!attendances.Any())
            {
                return 0;
            }

            var present = attendances.Count(x => x.Status == AttendanceStatus.Present);

            return Math.Round(
                (double)present / attendances.Count * 100,
                2);
        }

        /// <inheritdoc/>
       

        private static (
    int Total,
    int Present,
    int Absent,
    int Late,
    int Leave,
    double Percentage)

        CalculateAttendanceSummary(IEnumerable<StudentAttendance> attendances)
        {
            var attendanceList = attendances.ToList();

            var total = attendanceList.Count;

            var present = attendanceList.Count(x => x.Status == AttendanceStatus.Present);

            var absent = attendanceList.Count(x => x.Status == AttendanceStatus.Absent);

            var late = attendanceList.Count(x => x.Status == AttendanceStatus.Late);

            var leave = attendanceList.Count(x => x.Status == AttendanceStatus.Leave);

            var percentage = total == 0
                ? 0
                : Math.Round((double)present / total * 100, 2);

            return (
                total,
                present,
                absent,
                late,
                leave,
                percentage);
        }
    }
}
