using SchoolERP.Domain.Common;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents an academic year/session (e.g. 2025-2026).</summary>
public class AcademicYear : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public ICollection<FeeStructure> FeeStructures { get; set; } = new List<FeeStructure>();
    public ICollection<StudentFeeConcession> StudentFeeConcessions { get; set; } = new List<StudentFeeConcession>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<LateFineRule> LateFineRules { get; set; } = new List<LateFineRule>();
}
