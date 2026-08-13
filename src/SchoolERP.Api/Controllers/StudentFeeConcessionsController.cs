using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.StudentFeeConcession.DTOs;
using SchoolERP.Application.Features.StudentFeeConcession.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class StudentFeeConcessionsController : ControllerBase
    {
        private readonly IStudentFeeConcessionService _concessionService;

        public StudentFeeConcessionsController(IStudentFeeConcessionService concessionService)
        {
            _concessionService = concessionService;
        }

        /// <summary>Get all concessions for a specific student.</summary>
        [HttpGet("student/{studentId:int}")]
        [PermissionAuthorize(PermissionNames.ConcessionView)]
        [ProducesResponseType(typeof(IReadOnlyList<StudentFeeConcessionListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<StudentFeeConcessionListDto>>> GetByStudent(
            int studentId, CancellationToken cancellationToken)
        {
            return Ok(await _concessionService.GetByStudentIdAsync(studentId, cancellationToken));
        }

        /// <summary>Get all concessions awaiting approval (approver's queue).</summary>
        [HttpGet("pending-approvals")]
        [PermissionAuthorize(PermissionNames.ConcessionApprove)]
        [ProducesResponseType(typeof(IReadOnlyList<StudentFeeConcessionListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<StudentFeeConcessionListDto>>> GetPendingApprovals(
            CancellationToken cancellationToken)
        {
            return Ok(await _concessionService.GetPendingApprovalsAsync(cancellationToken));
        }

        /// <summary>Create a concession request (auto-approved if RequiresApproval is false).</summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.ConcessionCreate)]
        [ProducesResponseType(typeof(StudentFeeConcessionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StudentFeeConcessionDto>> Create(
            [FromBody] CreateStudentFeeConcessionDto request, CancellationToken cancellationToken)
        {
            var concession = await _concessionService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetByStudent), new { studentId = concession.StudentId }, concession);
        }

        /// <summary>Update a concession's terms.</summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.ConcessionEdit)]
        [ProducesResponseType(typeof(StudentFeeConcessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentFeeConcessionDto>> Update(
            int id, [FromBody] UpdateStudentFeeConcessionDto request, CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest("Route Id and body Id must match.");
            return Ok(await _concessionService.UpdateAsync(id, request, cancellationToken));
        }

        /// <summary>Approve a pending concession (state-transition action — sets ApprovedBy/ApprovedAt from the current employee).</summary>
        [HttpPost("{id:int}/approve")]
        [PermissionAuthorize(PermissionNames.ConcessionApprove)]
        [ProducesResponseType(typeof(StudentFeeConcessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentFeeConcessionDto>> Approve(
            int id, CancellationToken cancellationToken)
        {
            var request = new ApproveConcessionDto { ConcessionId = id };
            return Ok(await _concessionService.ApproveAsync(request, cancellationToken));
        }

        /// <summary>Soft-delete a concession.</summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.ConcessionDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _concessionService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
