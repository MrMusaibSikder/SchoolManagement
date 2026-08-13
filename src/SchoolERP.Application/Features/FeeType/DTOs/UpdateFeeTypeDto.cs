using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.FeeType.DTOs;

/// <summary>
/// Input model for updating an existing FeeType.
/// </summary>
public class UpdateFeeTypeDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int FeeCategoryId { get; set; }

    public FeeFrequency Frequency { get; set; }

    public bool IsMandatory { get; set; }

    public bool IsRefundable { get; set; }

    public bool IsActive { get; set; }

    /// <summary>
    /// Due day of the month (1-31). Null for OneTime fees.
    /// </summary>
    public int? DefaultDueDayOfMonth { get; set; }

    public int DefaultGracePeriodDays { get; set; }
}