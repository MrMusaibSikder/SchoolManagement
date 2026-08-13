using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.FeeType.DTOs;

/// <summary>
/// Input model for creating a new FeeType.
/// </summary>
public class CreateFeeTypeDto
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int FeeCategoryId { get; set; }

    public FeeFrequency Frequency { get; set; }

    public bool IsMandatory { get; set; } = true;

    public bool IsRefundable { get; set; }

    /// <summary>
    /// Due day of the month (1-31). Null for OneTime fees.
    /// </summary>
    public int? DefaultDueDayOfMonth { get; set; }

    /// <summary>
    /// Number of grace days allowed after the due date.
    /// </summary>
    public int DefaultGracePeriodDays { get; set; } = 5;
}