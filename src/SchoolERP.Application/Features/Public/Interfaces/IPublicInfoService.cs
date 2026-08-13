using SchoolERP.Application.Features.Public.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Public.Interfaces
{
    public interface IPublicInfoService
    {
        Task<PublicSchoolInfoDto> GetSchoolInfoAsync(CancellationToken cancellationToken = default);
        Task<PublicStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PublicNoticeDto>> GetPublicNoticesAsync(int take = 5, CancellationToken cancellationToken = default);
    }
}
