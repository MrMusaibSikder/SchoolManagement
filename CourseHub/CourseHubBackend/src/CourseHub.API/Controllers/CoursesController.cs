using CourseHub.API.Security;
using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Features.Courses;
using CourseHub.Application.Features.Courses.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers;

/// <summary>
/// Phase 12: admin Courses CRUD. All actions require the matching
/// "courses.*" permission — SuperAdmin passes automatically (seeded with
/// every permission, see DatabaseSeeder), everyone else needs the
/// permission explicitly assigned to their role via
/// RolePermissionsController.
/// </summary>
[Route("api/admin/courses")]
public class CoursesController : ApiControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IValidator<CreateCourseRequest> _createValidator;
    private readonly IValidator<UpdateCourseRequest> _updateValidator;
    private readonly IValidator<UpdateCourseThumbnailRequest> _updateThumbnailValidator;

    public CoursesController(
        ICourseService courseService,
        IValidator<CreateCourseRequest> createValidator,
        IValidator<UpdateCourseRequest> updateValidator,
        IValidator<UpdateCourseThumbnailRequest> updateThumbnailValidator)
    {
        _courseService = courseService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _updateThumbnailValidator = updateThumbnailValidator;
    }

    /// <summary>
    /// Every course regardless of active/public status (unlike
    /// GET /api/public/courses) — this is the admin management screen.
    /// </summary>
    [HttpGet]
    [HasPermission("courses.view")]
    [ProducesResponseType(typeof(PagedResult<CourseResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CourseResponse>>> Search(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _courseService.SearchAsync(search, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("courses.view")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var course = await _courseService.GetByIdAsync(id, cancellationToken);
        return Ok(course);
    }

    [HttpPost]
    [HasPermission("courses.create")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CourseResponse>> Create(CreateCourseRequest request, CancellationToken cancellationToken)
    {
        var validationError = await ValidateAsync(_createValidator, request, cancellationToken);
        if (validationError is not null)
        {
            return validationError;
        }

        var course = await _courseService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("courses.update")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseResponse>> Update(Guid id, UpdateCourseRequest request, CancellationToken cancellationToken)
    {
        var validationError = await ValidateAsync(_updateValidator, request, cancellationToken);
        if (validationError is not null)
        {
            return validationError;
        }

        var course = await _courseService.UpdateAsync(id, request, cancellationToken);
        return Ok(course);
    }

    [HttpPut("{id:guid}/thumbnail")]
    [HasPermission("courses.update")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseResponse>> UpdateThumbnail(Guid id, UpdateCourseThumbnailRequest request, CancellationToken cancellationToken)
    {
        var validationError = await ValidateAsync(_updateThumbnailValidator, request, cancellationToken);
        if (validationError is not null)
        {
            return validationError;
        }

        var course = await _courseService.UpdateThumbnailAsync(id, request, cancellationToken);
        return Ok(course);
    }

    [HttpPost("{id:guid}/activate")]
    [HasPermission("courses.update")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseResponse>> Activate(Guid id, CancellationToken cancellationToken)
    {
        var course = await _courseService.ActivateAsync(id, cancellationToken);
        return Ok(course);
    }

    [HttpPost("{id:guid}/deactivate")]
    [HasPermission("courses.update")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseResponse>> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var course = await _courseService.DeactivateAsync(id, cancellationToken);
        return Ok(course);
    }

    [HttpPost("{id:guid}/publish")]
    [HasPermission("courses.update")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseResponse>> Publish(Guid id, CancellationToken cancellationToken)
    {
        var course = await _courseService.PublishAsync(id, cancellationToken);
        return Ok(course);
    }

    [HttpPost("{id:guid}/unpublish")]
    [HasPermission("courses.update")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseResponse>> Unpublish(Guid id, CancellationToken cancellationToken)
    {
        var course = await _courseService.UnpublishAsync(id, cancellationToken);
        return Ok(course);
    }

    /// <summary>
    /// Soft delete (deactivates the course) — see CourseService.DeleteAsync
    /// for why this never removes the row.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [HasPermission("courses.delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _courseService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
