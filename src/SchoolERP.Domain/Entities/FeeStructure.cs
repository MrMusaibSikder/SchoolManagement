using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents the fee amount applicable for a fee type in a given class.</summary>
public class FeeStructure : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int AcademicYearId { get; set; }
    public AcademicYear AcademicYear { get; set; } = null!;

    public int SchoolClassId { get; set; }
    public SchoolClass SchoolClass { get; set; } = null!;

    public int? SectionId { get; set; }
    public Section? Section { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsTemplate { get; set; } // Can be cloned

    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    public int? ClonedFromId { get; set; }
    public FeeStructure? ClonedFrom { get; set; }

    public ICollection<FeeStructureItem> FeeStructureItems { get; set; } = new List<FeeStructureItem>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

