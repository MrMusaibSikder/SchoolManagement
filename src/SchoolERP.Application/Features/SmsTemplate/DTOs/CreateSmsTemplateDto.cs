namespace SchoolERP.Application.Features.SmsTemplate.DTOs;

/// <summary>Input model for creating a new SmsTemplate record.</summary>
public class CreateSmsTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    /// <summary>Whether the template should be active immediately. Defaults to <c>true</c>.</summary>
    public bool IsActive { get; set; } = true;
}
