using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Features.Public.DTOs;
using SchoolERP.Application.Features.Public.Interfaces;

namespace SchoolERP.Api.Controllers
{
    [Route("api/public")]
    [ApiController]
   
    public class PublicController : ControllerBase
    {
        private readonly IPublicInfoService _publicInfoService;

        public PublicController(IPublicInfoService publicInfoService)
        {
            _publicInfoService = publicInfoService;
        }

        [HttpGet("school-info")]
        [ProducesResponseType(typeof(PublicSchoolInfoDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<PublicSchoolInfoDto>> GetSchoolInfo(CancellationToken cancellationToken)
            => Ok(await _publicInfoService.GetSchoolInfoAsync(cancellationToken));

        [HttpGet("stats")]
        [ProducesResponseType(typeof(PublicStatsDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<PublicStatsDto>> GetStats(CancellationToken cancellationToken)
            => Ok(await _publicInfoService.GetStatsAsync(cancellationToken));

        [HttpGet("notices")]
        [ProducesResponseType(typeof(IReadOnlyList<PublicNoticeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<PublicNoticeDto>>> GetNotices(
            [FromQuery] int take = 5, CancellationToken cancellationToken = default)
            => Ok(await _publicInfoService.GetPublicNoticesAsync(take, cancellationToken));
    }
}
