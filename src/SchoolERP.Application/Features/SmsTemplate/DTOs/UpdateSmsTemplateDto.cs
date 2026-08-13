namespace SchoolERP.Application.Features.SmsTemplate.DTOs;

/// <summary>Input model for updating an existing SmsTemplate record.</summary>
public class UpdateSmsTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    /// <summary>Whether the template is active.</summary>
    public bool IsActive { get; set; }
}
