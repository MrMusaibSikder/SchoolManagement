using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.ExamResult.DTOs;
using SchoolERP.Application.Features.ExamResult.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Aggregate exam result calculation, ranking, publishing, and reporting:
    /// student mark sheets, tabulation sheets, merit lists, failed/top
    /// students, subject statistics, grade distribution and the exam result
    /// dashboard. Contains no marks entry logic — see
    /// <see cref="MarkEntryController"/> for that.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ExamResultController : ControllerBase
    {
        private readonly IExamResultService _examResultService;

        /// <summary>Initializes a new instance of <see cref="ExamResultController"/>.</summary>
        public ExamResultController(IExamResultService examResultService)
        {
            _examResultService = examResultService;
        }

        /// <summary>
        /// (Re)calculate the aggregate result for every student with submitted
        /// marks in this exam, and recompute Merit/Class/Section positions.
        /// </summary>
        [HttpPost("exam/{examId:int}/calculate")]
        [PermissionAuthorize(PermissionNames.ResultCalculate)]
        [ProducesResponseType(typeof(IReadOnlyList<ExamResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<ExamResultDto>>> Calculate(int examId, CancellationToken cancellationToken)
        {
            var result = await _examResultService.CalculateAsync(examId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Publish every calculated result for an exam, locking the underlying
        /// mark entries.
        /// </summary>
        [HttpPost("exam/{examId:int}/publish")]
        [PermissionAuthorize(PermissionNames.ResultPublish)]
        [ProducesResponseType(typeof(IReadOnlyList<ExamResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyList<ExamResultDto>>> Publish(int examId, CancellationToken cancellationToken)
        {
            var result = await _examResultService.PublishAsync(examId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Admin-only: unpublish an exam's results and unlock the underlying mark entries.
        /// </summary>
        [HttpPost("exam/{examId:int}/unpublish")]
        [PermissionAuthorize(PermissionNames.ResultUnlock)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Unpublish(int examId, CancellationToken cancellationToken)
        {
            await _examResultService.UnpublishAsync(examId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Get every aggregate result for an exam, optionally restricted to a class.
        /// </summary>
        [HttpGet("exam/{examId:int}")]
        [PermissionAuthorize(PermissionNames.ResultView)]
        [ProducesResponseType(typeof(IReadOnlyList<ExamResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ExamResultDto>>> GetByExam(
            int examId,
            [FromQuery] int? classId,
            CancellationToken cancellationToken)
        {
            var result = await _examResultService.GetByExamAsync(examId, classId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a student's full result (mark sheet: summary + subject breakdown) for one exam.
        /// </summary>
        [HttpGet("student/{studentId:int}/exam/{examId:int}")]
        [PermissionAuthorize(PermissionNames.ResultView)]
        [ProducesResponseType(typeof(StudentExamResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentExamResultDto>> GetStudentResult(int studentId, int examId, CancellationToken cancellationToken)
        {
            var result = await _examResultService.GetStudentResultAsync(studentId, examId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the full subject-by-student tabulation sheet for a class within an exam.
        /// </summary>
        [HttpGet("exam/{examId:int}/tabulation/class/{classId:int}")]
        [PermissionAuthorize(PermissionNames.ResultView)]
        [ProducesResponseType(typeof(TabulationSheetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TabulationSheetDto>> GetTabulationSheet(int examId, int classId, CancellationToken cancellationToken)
        {
            var result = await _examResultService.GetTabulationSheetAsync(examId, classId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the ranked merit list for a class within an exam.
        /// </summary>
        [HttpGet("exam/{examId:int}/merit/class/{classId:int}")]
        [PermissionAuthorize(PermissionNames.ResultView)]
        [ProducesResponseType(typeof(IReadOnlyList<MeritEntryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<MeritEntryDto>>> GetClassMeritList(int examId, int classId, CancellationToken cancellationToken)
        {
            var result = await _examResultService.GetClassMeritListAsync(examId, classId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the ranked merit list for a section within an exam.
        /// </summary>
        [HttpGet("exam/{examId:int}/merit/section/{sectionId:int}")]
        [PermissionAuthorize(PermissionNames.ResultView)]
        [ProducesResponseType(typeof(IReadOnlyList<MeritEntryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<MeritEntryDto>>> GetSectionMeritList(int examId, int sectionId, CancellationToken cancellationToken)
        {
            var result = await _examResultService.GetSectionMeritListAsync(examId, sectionId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get every student who failed the exam, optionally restricted to a class.
        /// </summary>
        [HttpGet("exam/{examId:int}/failed")]
        [PermissionAuthorize(PermissionNames.ResultView)]
        [ProducesResponseType(typeof(IReadOnlyList<MeritEntryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<MeritEntryDto>>> GetFailedStudents(
            int examId,
            [FromQuery] int? classId,
            CancellationToken cancellationToken)
        {
            var result = await _examResultService.GetFailedStudentsAsync(examId, classId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the top-performing students for the exam, optionally restricted to a class.
        /// </summary>
        [HttpGet("exam/{examId:int}/top")]
        [PermissionAuthorize(PermissionNames.ResultView)]
        [ProducesResponseType(typeof(IReadOnlyList<MeritEntryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<MeritEntryDto>>> GetTopStudents(
            int examId,
            [FromQuery] int? classId,
            [FromQuery] int count = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _examResultService.GetTopStudentsAsync(examId, classId, count, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get highest/lowest/average marks and pass rate for every subject of an exam.
        /// </summary>
        [HttpGet("exam/{examId:int}/subject-statistics")]
        [PermissionAuthorize(PermissionNames.ResultView)]
        [ProducesResponseType(typeof(IReadOnlyList<SubjectStatisticsDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SubjectStatisticsDto>>> GetSubjectStatistics(int examId, CancellationToken cancellationToken)
        {
            var result = await _examResultService.GetSubjectStatisticsAsync(examId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the number (and percentage) of students achieving each grade in the exam.
        /// </summary>
        [HttpGet("exam/{examId:int}/grade-distribution")]
        [PermissionAuthorize(PermissionNames.ResultView)]
        [ProducesResponseType(typeof(IReadOnlyList<GradeDistributionItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<GradeDistributionItemDto>>> GetGradeDistribution(int examId, CancellationToken cancellationToken)
        {
            var result = await _examResultService.GetGradeDistributionAsync(examId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get result-processing progress and outcome statistics for the exam dashboard.
        /// </summary>
        [HttpGet("exam/{examId:int}/dashboard")]
        [PermissionAuthorize(PermissionNames.ResultView)]
        [ProducesResponseType(typeof(ExamResultDashboardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamResultDashboardDto>> GetDashboard(int examId, CancellationToken cancellationToken)
        {
            var result = await _examResultService.GetDashboardAsync(examId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Set the teacher and/or guardian-facing remark for a student's
        /// result in this exam. Pass null for a remark to leave it unchanged.
        /// </summary>
        [HttpPatch("student/{studentId:int}/exam/{examId:int}/remarks")]
        [PermissionAuthorize(PermissionNames.ResultPublish)]
        [ProducesResponseType(typeof(ExamResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamResultDto>> SetRemarks(
            int studentId,
            int examId,
            [FromQuery] string? teacherRemarks,
            [FromQuery] string? guardianRemarks,
            CancellationToken cancellationToken)
        {
            var result = await _examResultService.SetRemarksAsync(studentId, examId, teacherRemarks, guardianRemarks, cancellationToken);
            return Ok(result);
        }
    }
}
