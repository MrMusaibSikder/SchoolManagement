using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.Transcript.DTOs;
using SchoolERP.Application.Features.Transcript.Interfaces;
using SchoolERP.Domain.Constants;
using SchoolERP.Infrastructure.Services;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Generates printable student transcripts: multi-academic-year history,
    /// exam-wise and subject-wise breakdowns, GPA/position trends, attendance
    /// summary, CGPA, and teacher/principal remarks. Returns presentation-
    /// neutral DTOs only — no PDF generation.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TranscriptController : ControllerBase
    {
        private readonly ITranscriptService _transcriptService;
        private readonly ITranscriptPdfService _transcriptPdfService;

        /// <summary>Initializes a new instance of <see cref="TranscriptController"/>.</summary>
        public TranscriptController(ITranscriptService transcriptService, ITranscriptPdfService transcriptPdfService)
        {
            _transcriptService = transcriptService;
            _transcriptPdfService = transcriptPdfService;

        }

        /// <summary>
        /// Get a student's full, multi-academic-year transcript: every
        /// published exam and year-end result, GPA/position trends,
        /// attendance summary, and overall CGPA.
        /// </summary>
        [HttpGet("student/{studentId:int}")]
        [PermissionAuthorize(PermissionNames.TranscriptView)]
        [ProducesResponseType(typeof(TranscriptDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TranscriptDto>> GetStudentTranscript(int studentId, CancellationToken cancellationToken)
        {
            var result = await _transcriptService.GetStudentTranscriptAsync(studentId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a student's transcript scoped to a single academic year.
        /// </summary>
        [HttpGet("student/{studentId:int}/academic-year/{academicYearId:int}")]
        [PermissionAuthorize(PermissionNames.TranscriptView)]
        [ProducesResponseType(typeof(TranscriptDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TranscriptDto>> GetAcademicYearTranscript(int studentId, int academicYearId, CancellationToken cancellationToken)
        {
            var result = await _transcriptService.GetAcademicYearTranscriptAsync(studentId, academicYearId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("student/{studentId:int}/pdf")]
        [PermissionAuthorize(PermissionNames.TranscriptView)]
        public async Task<IActionResult> GetStudentTranscriptPdf(int studentId, CancellationToken ct)
        {
            var bytes = await _transcriptPdfService.GenerateStudentTranscriptPdfAsync(studentId, ct);
            return File(bytes, "application/pdf", $"transcript-{studentId}.pdf");
        }

        [HttpGet("student/{studentId:int}/academic-year/{academicYearId:int}/pdf")]
        [PermissionAuthorize(PermissionNames.TranscriptView)]
        public async Task<IActionResult> GetAcademicYearTranscriptPdf(int studentId, int academicYearId, CancellationToken ct)
        {
            var bytes = await _transcriptPdfService.GenerateAcademicYearTranscriptPdfAsync(studentId, academicYearId, ct);
            return File(bytes, "application/pdf", $"transcript-{studentId}-{academicYearId}.pdf");
        }
    }
}
