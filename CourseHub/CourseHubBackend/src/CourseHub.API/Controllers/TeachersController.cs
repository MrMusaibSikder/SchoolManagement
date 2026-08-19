using CourseHub.API.Security;
using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Features.Teachers;
using CourseHub.Application.Features.Teachers.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers;

/// <summary>
/// Phase 12: admin Teachers CRUD. A Teacher profile always promotes an
/// existing User (see CreateTeacherRequest) — there is no standalone
/// "create a teacher from scratch" endpoint.
/// </summary>
[Route("api/admin/teachers")]
public class TeachersController : ApiControllerBase
{
    private readonly ITeacherService _teacherService;
    private readonly IValidator<CreateTeacherRequest> _createValidator;
    private readonly IValidator<UpdateTeacherProfileRequest> _updateProfileValidator;
    private readonly IValidator<UpdateTeacherContactRequest> _updateContactValidator;
    private readonly IValidator<UpdateTeacherProfileImageRequest> _updateProfileImageValidator;

    public TeachersController(
        ITeacherService teacherService,
        IValidator<CreateTeacherRequest> createValidator,
        IValidator<UpdateTeacherProfileRequest> updateProfileValidator,
        IValidator<UpdateTeacherContactRequest> updateContactValidator,
        IValidator<UpdateTeacherProfileImageRequest> updateProfileImageValidator)
    {
        _teacherService = teacherService;
        _createValidator = createValidator;
        _updateProfileValidator = updateProfileValidator;
        _updateContactValidator = updateContactValidator;
        _updateProfileImageValidator = updateProfileImageValidator;
    }

    /// <summary>
    /// Every teacher regardless of active/public status (unlike
    /// GET /api/public/teachers) — this is the admin management screen.
    /// </summary>
    [HttpGet]
    [HasPermission("teachers.view")]
    [ProducesResponseType(typeof(PagedResult<TeacherResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<TeacherResponse>>> Search(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _teacherService.SearchAsync(search, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("teachers.view")]
    [ProducesResponseType(typeof(TeacherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeacherResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var teacher = await _teacherService.GetByIdAsync(id, cancellationToken);
        return Ok(teacher);
    }

    /// <summary>
    /// Promotes an existing User (who must already hold the Teacher
    /// role) into a Teacher profile.
    /// </summary>
    [HttpPost]
    [HasPermission("teachers.create")]
    [ProducesResponseType(typeof(TeacherResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeacherResponse>> Create(CreateTeacherRequest request, CancellationToken cancellationToken)
    {
        
        if (!await ValidateAsync(
             _createValidator,
             request,
             cancellationToken))
        {
            return ValidationError();
        }

        var teacher = await _teacherService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, teacher);
    }

    [HttpPut("{id:guid}/profile")]
    [HasPermission("teachers.update")]
    [ProducesResponseType(typeof(TeacherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeacherResponse>> UpdateProfile(Guid id, UpdateTeacherProfileRequest request, CancellationToken cancellationToken)
    {
        if (!await ValidateAsync(
             _updateProfileValidator,
             request,
             cancellationToken))
        {
            return ValidationError();
        }


        var teacher = await _teacherService.UpdateProfileAsync(id, request, cancellationToken);
        return Ok(teacher);
    }

    [HttpPut("{id:guid}/contact")]
    [HasPermission("teachers.update")]
    [ProducesResponseType(typeof(TeacherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeacherResponse>> UpdateContact(Guid id, UpdateTeacherContactRequest request, CancellationToken cancellationToken)
    {
        if (!await ValidateAsync(
            _updateContactValidator,
            request,
            cancellationToken))
        {
            return ValidationError();
        }


        var teacher = await _teacherService.UpdateContactAsync(id, request, cancellationToken);
        return Ok(teacher);
    }

    [HttpPut("{id:guid}/profile-image")]
    [HasPermission("teachers.update")]
    [ProducesResponseType(typeof(TeacherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeacherResponse>> UpdateProfileImage(Guid id, UpdateTeacherProfileImageRequest request, CancellationToken cancellationToken)
    {
        if (!await ValidateAsync(
           _updateProfileImageValidator,
           request,
           cancellationToken))
        {
            return ValidationError();
        }

        var teacher = await _teacherService.UpdateProfileImageAsync(id, request, cancellationToken);
        return Ok(teacher);
    }

    [HttpPost("{id:guid}/activate")]
    [HasPermission("teachers.update")]
    [ProducesResponseType(typeof(TeacherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeacherResponse>> Activate(Guid id, CancellationToken cancellationToken)
    {
        var teacher = await _teacherService.ActivateAsync(id, cancellationToken);
        return Ok(teacher);
    }

    [HttpPost("{id:guid}/deactivate")]
    [HasPermission("teachers.update")]
    [ProducesResponseType(typeof(TeacherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeacherResponse>> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var teacher = await _teacherService.DeactivateAsync(id, cancellationToken);
        return Ok(teacher);
    }

    [HttpPost("{id:guid}/publish-profile")]
    [HasPermission("teachers.update")]
    [ProducesResponseType(typeof(TeacherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeacherResponse>> PublishProfile(Guid id, CancellationToken cancellationToken)
    {
        var teacher = await _teacherService.MakeProfilePublicAsync(id, cancellationToken);
        return Ok(teacher);
    }

    [HttpPost("{id:guid}/unpublish-profile")]
    [HasPermission("teachers.update")]
    [ProducesResponseType(typeof(TeacherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeacherResponse>> UnpublishProfile(Guid id, CancellationToken cancellationToken)
    {
        var teacher = await _teacherService.MakeProfilePrivateAsync(id, cancellationToken);
        return Ok(teacher);
    }

    /// <summary>
    /// Soft delete (deactivates the teacher) — see TeacherService.DeleteAsync.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [HasPermission("teachers.delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _teacherService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
