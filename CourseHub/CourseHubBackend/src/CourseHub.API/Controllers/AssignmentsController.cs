using CourseHub.API.Security;
using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Features.Assignments;
using CourseHub.Application.Features.Assignments.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers
{
    [Route("api/admin/assignments")]
    public class AssignmentsController : ApiControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentsController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        /// <summary>
        /// Returns all assignments for the admin management screen.
        /// Can optionally be filtered by CourseId.
        /// </summary>
        [HttpGet]
        [HasPermission("assignments.view")]
        [ProducesResponseType(typeof(PagedResult<AssignmentResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<AssignmentResponse>>> Search(
            [FromQuery] Guid? courseId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _assignmentService.SearchAsync(
                courseId,
                page,
                pageSize,
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [HasPermission("assignments.view")]
        [ProducesResponseType(typeof(AssignmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AssignmentResponse>> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var assignment = await _assignmentService.GetByIdAsync(
                id,
                cancellationToken);

            return Ok(assignment);
        }

        [HttpPost]
        [HasPermission("assignments.create")]
        [ProducesResponseType(typeof(AssignmentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AssignmentResponse>> Create(
            CreateAssignmentRequest request,
            CancellationToken cancellationToken)
        {
            var assignment = await _assignmentService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = assignment.Id },
                assignment);
        }

        [HttpPut("{id:guid}")]
        [HasPermission("assignments.update")]
        [ProducesResponseType(typeof(AssignmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AssignmentResponse>> Update(
            Guid id,
            UpdateAssignmentRequest request,
            CancellationToken cancellationToken)
        {
            var assignment = await _assignmentService.UpdateAsync(
                id,
                request,
                cancellationToken);

            return Ok(assignment);
        }

        [HttpPost("{id:guid}/activate")]
        [HasPermission("assignments.update")]
        [ProducesResponseType(typeof(AssignmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AssignmentResponse>> Activate(
            Guid id,
            CancellationToken cancellationToken)
        {
            var assignment = await _assignmentService.ActivateAsync(
                id,
                cancellationToken);

            return Ok(assignment);
        }

        [HttpPost("{id:guid}/deactivate")]
        [HasPermission("assignments.update")]
        [ProducesResponseType(typeof(AssignmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AssignmentResponse>> Deactivate(
            Guid id,
            CancellationToken cancellationToken)
        {
            var assignment = await _assignmentService.DeactivateAsync(
                id,
                cancellationToken);

            return Ok(assignment);
        }

        [HttpGet("{id:guid}/roster")]
        [HasPermission("submissions.grade")]
        [ProducesResponseType(typeof(IReadOnlyList<RosterEntryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<RosterEntryResponse>>> GetRoster(Guid id, CancellationToken cancellationToken)
        {
            var roster = await _assignmentService.GetRosterAsync(id, cancellationToken);
            return Ok(roster);
        }
    }
}
