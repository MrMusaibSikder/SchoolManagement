using SchoolERP.Application.Features.ProgressReport.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.ProgressReport.Interfaces
{
    public interface IProgressReportService
    {
        Task<ProgressReportDto> GetStudentProgressReportAsync(
            int studentId, int academicYearId, CancellationToken cancellationToken = default);
    }
}
