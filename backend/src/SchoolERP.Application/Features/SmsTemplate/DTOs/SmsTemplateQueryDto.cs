using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Features.SmsTemplate.DTOs;

/// <summary>
/// Query parameters for listing SmsTemplate records: free-text search over
/// name/message, active/inactive filtering, paging and sorting.
/// </summary>
public class SmsTemplateQueryDto : PagedQueryDto
{
    /// <summary>When supplied, restricts results to templates with this active state.</summary>
    public bool? IsActive { get; set; }
}
