using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Assignments;
using CourseHub.Application.Features.Assignments.Dtos;
using CourseHub.Application.Features.Batches;
using CourseHub.Application.Features.Batches.Dtos;
using CourseHub.Application.Features.Enrollments;
using CourseHub.Application.Features.Enrollments.Dtos;
using CourseHub.Application.Features.Me;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers;

/// <summary>
/// Self-service endpoints — "show me my own data". Only requires being
/// authenticated ([Authorize] with no policy/permission), unlike every
/// admin controller which requires a specific permission — a Student
/// doesn't need any special grant to see their own enrollments, the same
/// way any logged-in user can already call GET /api/auth/me.
/// </summary>
[Route("api/me")]
[Authorize]
public class MeController : ApiControllerBase
{
    private readonly IMeService _meService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStudentRepository _studentRepository;
    private readonly IAssignmentSubmissionService _assignmentSubmissionService;
    private readonly IBatchService _batchService;
    private readonly IEnrollmentService _enrollmentService;

    public MeController(
        IMeService meService,
        ICurrentUserService currentUserService,
        IStudentRepository studentRepository,
        IAssignmentSubmissionService assignmentSubmissionService,
        IBatchService batchService,
        IEnrollmentService enrollmentService)
    {
        _meService = meService;
        _currentUserService = currentUserService;
        _studentRepository = studentRepository;
        _assignmentSubmissionService = assignmentSubmissionService;
        _batchService = batchService;
        _enrollmentService = enrollmentService;
    }

    [HttpGet("enrollments")]
    [ProducesResponseType(typeof(PagedResult<EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<EnrollmentResponse>>> GetMyEnrollments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId!.Value;

        var result = await _meService.GetMyEnrollmentsAsync(
            userId,
            page,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Active batches under a course, for the "request enrollment" picker
    /// — a Student has no batches.view permission, so this proxies the
    /// same IBatchService.SearchAsync the admin screen uses, scoped down
    /// to one course and only active batches.
    /// </summary>
    [HttpGet("batches")]
    [ProducesResponseType(typeof(IReadOnlyList<BatchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<BatchResponse>>> GetAvailableBatches(
        [FromQuery] Guid courseId,
        CancellationToken cancellationToken = default)
    {
        var result = await _batchService.SearchAsync(null, courseId, 1, 100, cancellationToken);
        var activeOnly = result.Items.Where(b => b.IsActive).ToList();
        return Ok(activeOnly);
    }

    /// <summary>
    /// A student requesting to join a batch themselves — reuses
    /// IEnrollmentService.CreateAsync exactly as the admin "New
    /// enrollment" screen does, so it inherits the same validation
    /// (active student/batch, no duplicate, seat capacity) and always
    /// starts life as Status=Pending. A Teacher or Admin must still call
    /// POST /api/admin/enrollments/{id}/approve before the student is
    /// actually in the batch — this endpoint only ever creates the
    /// request, never approves it.
    /// </summary>
    [HttpPost("enrollments")]
    [ProducesResponseType(typeof(EnrollmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnrollmentResponse>> RequestEnrollment(
        RequestEnrollmentBody body,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId!.Value;

        var student = await _studentRepository.GetByUserIdAsync(userId, cancellationToken);

        if (student is null)
        {
            return NotFound("You don't have a student profile yet — ask an administrator to create one for you.");
        }

        var result = await _enrollmentService.CreateAsync(
            new CreateEnrollmentRequest(student.Id, body.BatchId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("assignments")]
    [ProducesResponseType(typeof(IReadOnlyList<MyAssignmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<MyAssignmentResponse>>> GetMyAssignments(
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId!.Value;

        var result = await _meService.GetMyAssignmentsAsync(
            userId,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("assignments/{id:guid}/submit")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(SubmissionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubmissionResponse>> SubmitAssignment(
        Guid id,
        [FromForm] SubmitAssignmentFormRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId!.Value;

        var student = await _studentRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (student is null)
        {
            return NotFound("You don't have a student profile yet — ask an administrator to create one for you.");
        }

        if (request.File is not null)
        {
            await using var stream = request.File.OpenReadStream();

            var result = await _assignmentSubmissionService.SubmitFileAsync(
                id,
                student.Id,
                stream,
                request.File.FileName,
                request.File.ContentType,
                cancellationToken);

            return Ok(result);
        }

        if (!string.IsNullOrWhiteSpace(request.TextContent))
        {
            var result = await _assignmentSubmissionService.SubmitTextAsync(
                id,
                student.Id,
                new SubmitTextAssignmentRequest(request.TextContent),
                cancellationToken);

            return Ok(result);
        }

        return BadRequest("Either TextContent or File must be provided.");
    }

    [HttpGet("submissions")]
    [ProducesResponseType(typeof(PagedResult<SubmissionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<SubmissionResponse>>> GetMySubmissions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId!.Value;

        var result = await _meService.GetMySubmissionsAsync(
            userId,
            page,
            pageSize,
            cancellationToken);

        return Ok(result);
    }
}

public class SubmitAssignmentFormRequest
{
    public string? TextContent { get; set; }

    public IFormFile? File { get; set; }
}

public record RequestEnrollmentBody(Guid BatchId);
