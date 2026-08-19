using CourseHub.API.Security;
using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Features.Students;
using CourseHub.Application.Features.Students.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers;

/// <summary>
/// Phase 12: admin Students CRUD. A Student profile always promotes an
/// existing User (see CreateStudentRequest) — same pattern as
/// TeachersController. There is no public Students listing endpoint
/// anywhere (privacy) — everything here requires authentication and the
/// matching "students.*" permission.
/// </summary>
[Route("api/admin/students")]
public class StudentsController : ApiControllerBase
{
    private readonly IStudentService _studentService;
    private readonly IValidator<CreateStudentRequest> _createValidator;
    private readonly IValidator<UpdateStudentProfileRequest> _updateProfileValidator;
    private readonly IValidator<UpdateStudentContactRequest> _updateContactValidator;
    private readonly IValidator<UpdateStudentGuardianRequest> _updateGuardianValidator;
    private readonly IValidator<UpdateStudentProfileImageRequest> _updateProfileImageValidator;

    public StudentsController(
        IStudentService studentService,
        IValidator<CreateStudentRequest> createValidator,
        IValidator<UpdateStudentProfileRequest> updateProfileValidator,
        IValidator<UpdateStudentContactRequest> updateContactValidator,
        IValidator<UpdateStudentGuardianRequest> updateGuardianValidator,
        IValidator<UpdateStudentProfileImageRequest> updateProfileImageValidator)
    {
        _studentService = studentService;
        _createValidator = createValidator;
        _updateProfileValidator = updateProfileValidator;
        _updateContactValidator = updateContactValidator;
        _updateGuardianValidator = updateGuardianValidator;
        _updateProfileImageValidator = updateProfileImageValidator;
    }

    [HttpGet]
    [HasPermission("students.view")]
    [ProducesResponseType(typeof(PagedResult<StudentResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<StudentResponse>>> Search(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _studentService.SearchAsync(search, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("students.view")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var student = await _studentService.GetByIdAsync(id, cancellationToken);
        return Ok(student);
    }

    /// <summary>
    /// Promotes an existing User (who must already hold the Student
    /// role) into a Student profile.
    /// </summary>
    [HttpPost]
    [HasPermission("students.create")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentResponse>> Create(CreateStudentRequest request, CancellationToken cancellationToken)
    {
        if (!await ValidateAsync(
             _createValidator,
             request,
             cancellationToken))
        {
            return ValidationError();
        }

        var student = await _studentService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
    }

    [HttpPut("{id:guid}/profile")]
    [HasPermission("students.update")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentResponse>> UpdateProfile(Guid id, UpdateStudentProfileRequest request, CancellationToken cancellationToken)
    {
        if (!await ValidateAsync(
             _updateProfileValidator,
             request,
             cancellationToken))
        {
            return ValidationError();
        }

        var student = await _studentService.UpdateProfileAsync(id, request, cancellationToken);
        return Ok(student);
    }

    [HttpPut("{id:guid}/contact")]
    [HasPermission("students.update")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentResponse>> UpdateContact(Guid id, UpdateStudentContactRequest request, CancellationToken cancellationToken)
    {
        if (!await ValidateAsync(
           _updateContactValidator,
           request,
           cancellationToken))
        {
            return ValidationError();
        }
        var student = await _studentService.UpdateContactAsync(id, request, cancellationToken);
        return Ok(student);
    }

    [HttpPut("{id:guid}/guardian")]
    [HasPermission("students.update")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentResponse>> UpdateGuardian(Guid id, UpdateStudentGuardianRequest request, CancellationToken cancellationToken)
    {
        if (!await ValidateAsync(
            _updateGuardianValidator,
            request,
            cancellationToken))
        {
            return ValidationError();
        }

        var student = await _studentService.UpdateGuardianAsync(id, request, cancellationToken);
        return Ok(student);
    }

    [HttpPut("{id:guid}/profile-image")]
    [HasPermission("students.update")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentResponse>> UpdateProfileImage(Guid id, UpdateStudentProfileImageRequest request, CancellationToken cancellationToken)
    {
        if (!await ValidateAsync(
           _updateProfileImageValidator,
           request,
           cancellationToken))
        {
            return ValidationError();
        }
        var student = await _studentService.UpdateProfileImageAsync(id, request, cancellationToken);
        return Ok(student);
    }

    [HttpPost("{id:guid}/activate")]
    [HasPermission("students.update")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentResponse>> Activate(Guid id, CancellationToken cancellationToken)
    {
        var student = await _studentService.ActivateAsync(id, cancellationToken);
        return Ok(student);
    }

    [HttpPost("{id:guid}/deactivate")]
    [HasPermission("students.update")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentResponse>> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var student = await _studentService.DeactivateAsync(id, cancellationToken);
        return Ok(student);
    }

    /// <summary>
    /// Toggles Student.IsProfilePublic. No current public endpoint reads
    /// this flag (there is deliberately no public students listing) —
    /// kept here for admin completeness/future use, matching the domain
    /// entity's existing MakeProfilePublic/MakeProfilePrivate methods.
    /// </summary>
    [HttpPost("{id:guid}/publish-profile")]
    [HasPermission("students.update")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentResponse>> PublishProfile(Guid id, CancellationToken cancellationToken)
    {
        var student = await _studentService.MakeProfilePublicAsync(id, cancellationToken);
        return Ok(student);
    }

    [HttpPost("{id:guid}/unpublish-profile")]
    [HasPermission("students.update")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentResponse>> UnpublishProfile(Guid id, CancellationToken cancellationToken)
    {
        var student = await _studentService.MakeProfilePrivateAsync(id, cancellationToken);
        return Ok(student);
    }

    /// <summary>
    /// Soft delete (deactivates the student) — see StudentService.DeleteAsync,
    /// which is also backed by a real FK (Enrollment.StudentId is Restrict),
    /// unlike Teacher's version of this same pattern.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [HasPermission("students.delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _studentService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
