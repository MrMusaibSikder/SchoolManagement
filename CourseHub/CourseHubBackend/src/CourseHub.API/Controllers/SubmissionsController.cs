using CourseHub.API.Security;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Assignments;
using CourseHub.Application.Features.Assignments.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers
{
    [Route("api/admin/submissions")]
    public class SubmissionsController : ApiControllerBase
    {
        private readonly IAssignmentSubmissionService _submissionService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITeacherRepository _teacherRepository;

        public SubmissionsController(
            IAssignmentSubmissionService submissionService,
            ICurrentUserService currentUserService,
            ITeacherRepository teacherRepository)
        {
            _submissionService = submissionService;
            _currentUserService = currentUserService;
            _teacherRepository = teacherRepository;
        }

        [HttpGet("assignment/{assignmentId:guid}/roster")]
        [HasPermission("submissions.grade")]
        [ProducesResponseType(
            typeof(IReadOnlyList<AssignmentRosterResponse>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<AssignmentRosterResponse>>> GetRoster(
            Guid assignmentId,
            CancellationToken cancellationToken)
        {
            var result = await _submissionService.GetRosterAsync(
                assignmentId,
                cancellationToken);

            return Ok(result);
        }

        [HttpPost("{submissionId:guid}/grade")]
        [HasPermission("submissions.grade")]
        [ProducesResponseType(
            typeof(SubmissionResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SubmissionResponse>> Grade(
            Guid submissionId,
            [FromBody] GradeSubmissionRequest request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId!.Value;

            var teacher = await _teacherRepository.GetByUserIdAsync(
                userId,
                cancellationToken);

            Guid? teacherId = teacher?.Id;

            var result = await _submissionService.GradeAsync(
                submissionId,
                teacherId,
                request,
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{submissionId:guid}/file")]
        [HasPermission("submissions.grade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadFile(
    Guid submissionId,
    CancellationToken cancellationToken)
        {
            var result = await _submissionService.DownloadFileAsync(
                submissionId,
                _currentUserService.UserId,
                cancellationToken);

            return File(
                result.Stream,
                result.ContentType,
                result.FileName);
        }
    }
}
