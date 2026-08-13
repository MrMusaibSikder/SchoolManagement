namespace SchoolERP.Application.Features.SmsTemplate.DTOs;

/// <summary>Read model returned to clients for a SmsTemplate record.</summary>
public class SmsTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    /// <summary>Whether this template is currently active and available for use.</summary>
    public bool IsActive { get; set; }

    /// <summary>UTC timestamp when the template was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>UTC timestamp when the template was last updated, if any.</summary>
    public DateTime? UpdatedAt { get; set; }
}
