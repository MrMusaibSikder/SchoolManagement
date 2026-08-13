using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.ProgressReport.DTOs;
using SchoolERP.Application.Features.ProgressReport.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Generates a student's intra-year progress report: exam-by-exam,
    /// subject-wise marks trend within a single academic year.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProgressReportController : ControllerBase
    {
        private readonly IProgressReportService _progressReportService;

        public ProgressReportController(IProgressReportService progressReportService)
        {
            _progressReportService = progressReportService;
        }

        /// <summary>
        /// Gets a student's exam-by-exam, subject-wise progress report for a
        /// single academic year.
        /// </summary>
        [HttpGet("student/{studentId:int}/academic-year/{academicYearId:int}")]
        [PermissionAuthorize(PermissionNames.ProgressReportView)]
        [ProducesResponseType(typeof(ProgressReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProgressReportDto>> GetStudentProgressReport(
            int studentId, int academicYearId, CancellationToken cancellationToken)
        {
            var result = await _progressReportService.GetStudentProgressReportAsync(studentId, academicYearId, cancellationToken);
            return Ok(result);
        }
    }
}