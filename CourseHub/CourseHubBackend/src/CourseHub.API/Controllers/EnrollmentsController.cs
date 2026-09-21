using CourseHub.API.Security;
using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Features.Enrollments;
using CourseHub.Application.Features.Enrollments.Dtos;
using CourseHub.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers;

/// <summary>
/// Phase 12: admin Enrollments CRUD — the last piece of the Phase 12
/// dependency chain (Student + Batch, both already built). Enrollment has
/// no IsActive/soft-delete flag of its own (see the Enrollment entity) —
/// its lifecycle is the Pending -> Active -> Completed state machine plus
/// Cancel, so DELETE below maps to Cancel rather than a separate concept.
/// </summary>
[Route("api/admin/enrollments")]
public class EnrollmentsController : ApiControllerBase
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly IValidator<CreateEnrollmentRequest> _createValidator;

    public EnrollmentsController(IEnrollmentService enrollmentService, IValidator<CreateEnrollmentRequest> createValidator)
    {
        _enrollmentService = enrollmentService;
        _createValidator = createValidator;
    }

    /// <summary>
    /// Optionally filtered by studentId, batchId, and/or status.
    /// </summary>
    [HttpGet]
    [HasPermission("enrollments.view")]
    [ProducesResponseType(typeof(PagedResult<EnrollmentResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<EnrollmentResponse>>> Search(
        [FromQuery] Guid? studentId,
        [FromQuery] Guid? batchId,
        [FromQuery] EnrollmentStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _enrollmentService.SearchAsync(studentId, batchId, status, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("enrollments.view")]
    [ProducesResponseType(typeof(EnrollmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnrollmentResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentService.GetByIdAsync(id, cancellationToken);
        return Ok(enrollment);
    }

    /// <summary>
    /// Enrolls an existing, active Student into an existing, active
    /// Batch. Starts as Status=Pending — call /approve next.
    /// </summary>
    [HttpPost]
    [HasPermission("enrollments.create")]
    [ProducesResponseType(typeof(EnrollmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnrollmentResponse>> Create(CreateEnrollmentRequest request, CancellationToken cancellationToken)
    {
        var validationError = await ValidateAsync(_createValidator, request, cancellationToken);
        if (validationError is not null)
        {
            return validationError;
        }

        var enrollment = await _enrollmentService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = enrollment.Id }, enrollment);
    }

    /// <summary>
    /// Pending -> Active.
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    [HasPermission("enrollments.update")]
    [ProducesResponseType(typeof(EnrollmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnrollmentResponse>> Approve(Guid id, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentService.ApproveAsync(id, cancellationToken);
        return Ok(enrollment);
    }

    /// <summary>
    /// Active -> Completed.
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    [HasPermission("enrollments.update")]
    [ProducesResponseType(typeof(EnrollmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnrollmentResponse>> Complete(Guid id, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentService.CompleteAsync(id, cancellationToken);
        return Ok(enrollment);
    }

    /// <summary>
    /// Pending/Active -> Cancelled.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [HasPermission("enrollments.update")]
    [ProducesResponseType(typeof(EnrollmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnrollmentResponse>> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentService.CancelAsync(id, cancellationToken);
        return Ok(enrollment);
    }

    /// <summary>
    /// Permanently deletes the row — only Cancelled/Completed enrollments
    /// qualify (EnrollmentService.DeleteAsync enforces this); a
    /// Pending/Active enrollment must be cancelled first via
    /// POST /{id}/cancel.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [HasPermission("enrollments.delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _enrollmentService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
