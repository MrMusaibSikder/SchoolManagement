using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>
/// A named, versioned set of exam weightings (e.g. "Mid Term 1 = 20%, ...")
/// used to compute a <see cref="FinalResult"/> for an academic year. Multiple
/// setups may exist per academic year (history/versioning); at most one is
/// <see cref="IsActive"/> per academic year at any time.
/// </summary>
public class ExamWeightSetup : BaseEntity
{
    public int AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    /// <summary>Display label for this weight configuration (e.g. "2026 Standard Weighting").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Whether this is the currently effective weighting for its academic year.</summary>
    public bool IsActive { get; set; }

    public ICollection<ExamWeightItem> Items { get; set; } = new List<ExamWeightItem>();
}
