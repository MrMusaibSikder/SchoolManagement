using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents a reusable SMS message template.</summary>
public class SmsTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    /// <summary>Whether this template is currently active and available for use.</summary>
    public bool IsActive { get; set; } = true;
}
