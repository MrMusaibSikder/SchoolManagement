using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents a category of fee (e.g. Tuition, Admission, Exam Fee).</summary>
public class FeeType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int FeeCategoryId { get; set; }
    public FeeCategory FeeCategory { get; set; } = null!;

    public FeeFrequency Frequency { get; set; }
    public bool IsMandatory { get; set; } = true;
    public bool IsRefundable { get; set; }
    public bool IsActive { get; set; } = true;

    public int? DefaultDueDayOfMonth { get; set; }
    public int DefaultGracePeriodDays { get; set; }

    public ICollection<FeeStructureItem> FeeStructureItems { get; set; } = new List<FeeStructureItem>();
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    public ICollection<StudentFeeConcession> StudentFeeConcessions { get; set; } = new List<StudentFeeConcession>();
    public ICollection<LateFineRule> LateFineRules { get; set; } = new List<LateFineRule>();
}
