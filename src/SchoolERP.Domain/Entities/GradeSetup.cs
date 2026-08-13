using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>
/// A single configurable grade band (e.g. "A+", GPA 5.00, 80-100%) for an
/// academic year. Replaces hardcoded grading logic so grading policy can
/// change year to year without a code deployment. When no rows exist for an
/// academic year, the system falls back to the built-in standard Bangladesh
/// GPA scale (see <c>GradeCalculator</c>) for full backward compatibility.
/// </summary>
public class GradeSetup : BaseEntity
{
    public int AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    /// <summary>Display label for this grade (e.g. "A+").</summary>
    public string GradeName { get; set; } = string.Empty;

    /// <summary>Grade point awarded for this band (e.g. 5.00).</summary>
    public decimal GradePoint { get; set; }

    /// <summary>Inclusive lower bound of raw marks this band applies to (mirrors <see cref="MinPercentage"/>; kept for setups that grade on raw marks instead of percentage).</summary>
    public decimal MinMarks { get; set; }

    /// <summary>Inclusive upper bound of raw marks this band applies to.</summary>
    public decimal MaxMarks { get; set; }

    /// <summary>Inclusive lower bound of percentage this band applies to.</summary>
    public decimal MinPercentage { get; set; }

    /// <summary>Inclusive upper bound of percentage this band applies to.</summary>
    public decimal MaxPercentage { get; set; }

    /// <summary>Whether achieving this grade counts as a fail (e.g. "F").</summary>
    public bool IsFail { get; set; }

    /// <summary>Display/evaluation order (also used to break ties when bands are checked). Lower sorts first (highest grade first is the recommended convention).</summary>
    public int DisplayOrder { get; set; }

    /// <summary>Whether this grade band is currently in effect for its academic year.</summary>
    public bool IsActive { get; set; } = true;
}
