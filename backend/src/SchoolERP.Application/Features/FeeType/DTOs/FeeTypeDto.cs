using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.FeeType.DTOs;

/// <summary>
/// Detailed FeeType information returned to clients.
/// </summary>
public class FeeTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int FeeCategoryId { get; set; }
    /// <summary>
    /// Category name for display purposes.
    /// </summary>
    public string? FeeCategoryName { get; set; }
    public FeeFrequency Frequency { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsRefundable { get; set; }
    public bool IsActive { get; set; }
    public int? DefaultDueDayOfMonth { get; set; }
    public int DefaultGracePeriodDays { get; set; }
    public DateTime CreatedAt { get; set; }
}
