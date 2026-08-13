using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.SmsTemplate.DTOs;

namespace SchoolERP.Application.Features.SmsTemplate.Interfaces;

/// <summary>
/// Business/service contract for SmsTemplate records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface ISmsTemplateService
{
    /// <summary>Retrieves every SmsTemplate record.</summary>
    Task<IReadOnlyList<SmsTemplateDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a search-filtered, paged, sorted list of SmsTemplate records.</summary>
    Task<PagedResult<SmsTemplateDto>> GetPagedAsync(SmsTemplateQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single SmsTemplate record by id, or null if it does not exist.</summary>
    Task<SmsTemplateDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new SmsTemplate record. Template names must be unique (case-insensitive).</summary>
    Task<SmsTemplateDto> CreateAsync(CreateSmsTemplateDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing SmsTemplate record. Template names must be unique (case-insensitive).</summary>
    Task<SmsTemplateDto> UpdateAsync(int id, UpdateSmsTemplateDto request, CancellationToken cancellationToken = default);

    /// <summary>Activates a template, making it available for use.</summary>
    Task<SmsTemplateDto> ActivateAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Deactivates a template, removing it from active use without deleting it.</summary>
    Task<SmsTemplateDto> DeactivateAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Renders a template's message with the supplied placeholder values substituted.</summary>
    Task<RenderedSmsTemplateDto> RenderAsync(int id, RenderSmsTemplateDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing SmsTemplate record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
