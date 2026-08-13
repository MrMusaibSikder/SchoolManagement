using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Features.SmsTemplate.DTOs;

/// <summary>Input model for previewing/rendering a template with real placeholder values.</summary>
public class RenderSmsTemplateDto
{
    /// <summary>The placeholder values to substitute into the template.</summary>
    public PlaceholderDataDto Data { get; set; } = new();
}
