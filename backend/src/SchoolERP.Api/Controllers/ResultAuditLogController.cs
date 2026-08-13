using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Application.Features.ResultAuditLog.DTOs;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Read-only access to the Result-management audit trail: every mark
    /// update, calculation, publish/unpublish, lock/unlock and rollback is
    /// recorded here with who performed it and when.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ResultAuditLogController : ControllerBase
    {
        private readonly IResultAuditService _resultAuditService;

        /// <summary>Initializes a new instance of <see cref="ResultAuditLogController"/>.</summary>
        public ResultAuditLogController(IResultAuditService resultAuditService)
        {
            _resultAuditService = resultAuditService;
        }

        /// <summary>
        /// Get the full audit history for a specific entity (e.g. "Result",
        /// "Exam", "AcademicYear"), most recent first.
        /// </summary>
        [HttpGet("{entityType}/{entityId:int}")]
        [PermissionAuthorize(PermissionNames.ResultAuditView)]
        [ProducesResponseType(typeof(IReadOnlyList<ResultAuditLogDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ResultAuditLogDto>>> GetHistory(
            string entityType,
            int entityId,
            CancellationToken cancellationToken)
        {
            var result = await _resultAuditService.GetHistoryAsync(entityType, entityId, cancellationToken);
            return Ok(result);
        }
    }
}
