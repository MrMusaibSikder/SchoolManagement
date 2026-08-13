using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.FinalResult.DTOs;
using SchoolERP.Application.Features.FinalResult.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Weighted, year-wide final result calculation, ranking, and publishing.
    /// Combines every exam result for a student across an academic year using
    /// the active <see cref="SchoolERP.Domain.Entities.ExamWeightSetup"/>.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FinalResultController : ControllerBase
    {
        private readonly IFinalResultService _finalResultService;

        /// <summary>Initializes a new instance of <see cref="FinalResultController"/>.</summary>
        public FinalResultController(IFinalResultService finalResultService)
        {
            _finalResultService = finalResultService;
        }

        /// <summary>
        /// (Re)calculate the weighted final result for every eligible student in
        /// an academic year, using its active weight setup, and recompute
        /// Merit/Class/Section positions.
        /// </summary>
        [HttpPost("academic-year/{academicYearId:int}/calculate")]
        [PermissionAuthorize(PermissionNames.FinalResultCalculate)]
        [ProducesResponseType(typeof(IReadOnlyList<FinalResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyList<FinalResultDto>>> Calculate(int academicYearId, CancellationToken cancellationToken)
        {
            var result = await _finalResultService.CalculateAsync(academicYearId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Publish every calculated final result for an academic year.
        /// </summary>
        [HttpPost("academic-year/{academicYearId:int}/publish")]
        [PermissionAuthorize(PermissionNames.FinalResultPublish)]
        [ProducesResponseType(typeof(IReadOnlyList<FinalResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyList<FinalResultDto>>> Publish(int academicYearId, CancellationToken cancellationToken)
        {
            var result = await _finalResultService.PublishAsync(academicYearId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Admin-only: unpublish an academic year's final results so corrections can be made.
        /// </summary>
        [HttpPost("academic-year/{academicYearId:int}/unpublish")]
        [PermissionAuthorize(PermissionNames.FinalResultUnlock)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Unpublish(int academicYearId, CancellationToken cancellationToken)
        {
            await _finalResultService.UnpublishAsync(academicYearId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Get every final result for an academic year, optionally restricted to a class.
        /// </summary>
        [HttpGet("academic-year/{academicYearId:int}")]
        [PermissionAuthorize(PermissionNames.FinalResultView)]
        [ProducesResponseType(typeof(IReadOnlyList<FinalResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<FinalResultDto>>> GetByAcademicYear(
            int academicYearId,
            [FromQuery] int? classId,
            CancellationToken cancellationToken)
        {
            var result = await _finalResultService.GetByAcademicYearAsync(academicYearId, classId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a single student's final result (with subject breakdown) for an academic year.
        /// </summary>
        [HttpGet("student/{studentId:int}/academic-year/{academicYearId:int}")]
        [PermissionAuthorize(PermissionNames.FinalResultView)]
        [ProducesResponseType(typeof(FinalResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FinalResultDto>> GetStudentFinalResult(int studentId, int academicYearId, CancellationToken cancellationToken)
        {
            var result = await _finalResultService.GetStudentFinalResultAsync(studentId, academicYearId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the ranked final-result merit list for a class within an academic year.
        /// </summary>
        [HttpGet("academic-year/{academicYearId:int}/merit/class/{classId:int}")]
        [PermissionAuthorize(PermissionNames.FinalResultView)]
        [ProducesResponseType(typeof(IReadOnlyList<MeritEntryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<MeritEntryDto>>> GetClassMeritList(int academicYearId, int classId, CancellationToken cancellationToken)
        {
            var result = await _finalResultService.GetClassMeritListAsync(academicYearId, classId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the ranked final-result merit list for a section within an academic year.
        /// </summary>
        [HttpGet("academic-year/{academicYearId:int}/merit/section/{sectionId:int}")]
        [PermissionAuthorize(PermissionNames.FinalResultView)]
        [ProducesResponseType(typeof(IReadOnlyList<MeritEntryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<MeritEntryDto>>> GetSectionMeritList(int academicYearId, int sectionId, CancellationToken cancellationToken)
        {
            var result = await _finalResultService.GetSectionMeritListAsync(academicYearId, sectionId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Set the teacher and/or principal remark shown on a student's
        /// transcript for the year. Pass null for a remark to leave it unchanged.
        /// </summary>
        [HttpPatch("student/{studentId:int}/academic-year/{academicYearId:int}/remarks")]
        [PermissionAuthorize(PermissionNames.FinalResultPublish)]
        [ProducesResponseType(typeof(FinalResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FinalResultDto>> SetRemarks(
            int studentId,
            int academicYearId,
            [FromQuery] string? teacherRemarks,
            [FromQuery] string? principalRemarks,
            CancellationToken cancellationToken)
        {
            var result = await _finalResultService.SetRemarksAsync(studentId, academicYearId, teacherRemarks, principalRemarks, cancellationToken);
            return Ok(result);
        }
    }
}
