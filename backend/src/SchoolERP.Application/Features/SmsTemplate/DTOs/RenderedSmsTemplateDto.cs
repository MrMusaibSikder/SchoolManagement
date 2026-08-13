namespace SchoolERP.Application.Features.SmsTemplate.DTOs;

/// <summary>Result of rendering a SmsTemplate with placeholder values substituted.</summary>
public class RenderedSmsTemplateDto
{
    public int TemplateId { get; set; }
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>The original, unmodified template text.</summary>
    public string RawMessage { get; set; } = string.Empty;

    /// <summary>The template text with every supplied placeholder substituted.</summary>
    public string RenderedMessage { get; set; } = string.Empty;
}
