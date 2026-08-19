using CourseHub.API.Security;
using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Features.Batches;
using CourseHub.Application.Features.Batches.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers;

/// <summary>
/// Phase 12: admin Batches CRUD. A Batch always belongs to an existing,
/// active Course (see CreateBatchRequest).
/// </summary>
[Route("api/admin/batches")]
public class BatchesController : ApiControllerBase
{
    private readonly IBatchService _batchService;
    private readonly IValidator<CreateBatchRequest> _createValidator;
    private readonly IValidator<UpdateBatchRequest> _updateValidator;
    private readonly IValidator<UpdateBatchScheduleRequest> _updateScheduleValidator;
    private readonly IValidator<UpdateBatchCapacityRequest> _updateCapacityValidator;

    public BatchesController(
        IBatchService batchService,
        IValidator<CreateBatchRequest> createValidator,
        IValidator<UpdateBatchRequest> updateValidator,
        IValidator<UpdateBatchScheduleRequest> updateScheduleValidator,
        IValidator<UpdateBatchCapacityRequest> updateCapacityValidator)
    {
        _batchService = batchService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _updateScheduleValidator = updateScheduleValidator;
        _updateCapacityValidator = updateCapacityValidator;
    }

    /// <summary>
    /// Optionally scoped to a single course via ?courseId= (e.g. "show me
    /// all batches of this course"). Returns every batch regardless of
    /// active status — admin management screen.
    /// </summary>
    [HttpGet]
    [HasPermission("batches.view")]
    [ProducesResponseType(typeof(PagedResult<BatchResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<BatchResponse>>> Search(
        [FromQuery] string? search,
        [FromQuery] Guid? courseId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _batchService.SearchAsync(search, courseId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("batches.view")]
    [ProducesResponseType(typeof(BatchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BatchResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var batch = await _batchService.GetByIdAsync(id, cancellationToken);
        return Ok(batch);
    }

    [HttpPost]
    [HasPermission("batches.create")]
    [ProducesResponseType(typeof(BatchResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BatchResponse>> Create(CreateBatchRequest request, CancellationToken cancellationToken)
    {
      
        if (!await ValidateAsync (_createValidator,request,cancellationToken))
        {
            return ValidationError();
        }

        var batch = await _batchService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = batch.Id }, batch);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("batches.update")]
    [ProducesResponseType(typeof(BatchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BatchResponse>> Update(Guid id, UpdateBatchRequest request, CancellationToken cancellationToken)
    {
        if (!await ValidateAsync(_updateValidator, request, cancellationToken))
        {
            return ValidationError();
        }

        var batch = await _batchService.UpdateAsync(id, request, cancellationToken);
        return Ok(batch);
    }

    [HttpPut("{id:guid}/schedule")]
    [HasPermission("batches.update")]
    [ProducesResponseType(typeof(BatchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BatchResponse>> UpdateSchedule(Guid id, UpdateBatchScheduleRequest request, CancellationToken cancellationToken)
    {
        if (!await ValidateAsync(
             _updateScheduleValidator,
             request,
             cancellationToken))
        {
            return ValidationError();
        }

        var batch = await _batchService.UpdateScheduleAsync(id, request, cancellationToken);
        return Ok(batch);
    }

    [HttpPut("{id:guid}/capacity")]
    [HasPermission("batches.update")]
    [ProducesResponseType(typeof(BatchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BatchResponse>> UpdateCapacity(Guid id, UpdateBatchCapacityRequest request, CancellationToken cancellationToken)
    {
        if (!await ValidateAsync(
             _updateCapacityValidator,
             request,
             cancellationToken))
        {
            return ValidationError();
        }

        var batch = await _batchService.UpdateCapacityAsync(id, request, cancellationToken);
        return Ok(batch);
    }

    [HttpPost("{id:guid}/activate")]
    [HasPermission("batches.update")]
    [ProducesResponseType(typeof(BatchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BatchResponse>> Activate(Guid id, CancellationToken cancellationToken)
    {
        var batch = await _batchService.ActivateAsync(id, cancellationToken);
        return Ok(batch);
    }

    [HttpPost("{id:guid}/deactivate")]
    [HasPermission("batches.update")]
    [ProducesResponseType(typeof(BatchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BatchResponse>> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var batch = await _batchService.DeactivateAsync(id, cancellationToken);
        return Ok(batch);
    }

    /// <summary>
    /// Soft delete (deactivates the batch) — see BatchService.DeleteAsync,
    /// backed by a real FK (Enrollment.BatchId is Restrict).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [HasPermission("batches.delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _batchService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
