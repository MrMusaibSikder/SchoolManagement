using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>
/// Aggregate, per-student result for a single <see cref="Exam"/>, computed
/// from every <see cref="Result"/> (subject mark entry) row belonging to that
/// student across the exam's schedules. One row per Student + Exam.
/// </summary>
public class ExamResult : BaseEntity
{
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public int ExamId { get; set; }
    public Exam? Exam { get; set; }

    /// <summary>Sum of (MarksObtained + GraceMarks) across every subject of the exam.</summary>
    public decimal TotalMarks { get; set; }

    /// <summary>Sum of FullMarks across every subject of the exam.</summary>
    public decimal TotalFullMarks { get; set; }

    /// <summary>TotalMarks / TotalFullMarks * 100.</summary>
    public decimal Percentage { get; set; }

    /// <summary>Average grade point across every subject (simple mean of subject GPAs).</summary>
    public decimal GPA { get; set; }

    /// <summary>Overall letter grade derived from <see cref="GPA"/>.</summary>
    public string Grade { get; set; } = string.Empty;

    /// <summary>False if the student failed any single subject (standard "F in one = fail overall" rule) or the overall percentage is below the pass threshold.</summary>
    public bool IsPassed { get; set; }

    /// <summary>1-based rank among every student who sat this exam (by GPA, then percentage). Null until calculated.</summary>
    public int? MeritPosition { get; set; }

    /// <summary>1-based rank within the student's class for this exam. Null until calculated.</summary>
    public int? ClassPosition { get; set; }

    /// <summary>1-based rank within the student's section for this exam. Null until calculated.</summary>
    public int? SectionPosition { get; set; }

    /// <summary>Whether this result has been published (visible to students/guardians) and locked.</summary>
    public bool IsPublished { get; set; }

    /// <summary>UTC timestamp the result was published, if any.</summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>Id of the User who published this result.</summary>
    public int? PublishedBy { get; set; }

    /// <summary>Optional subject/class teacher's remark for this exam.</summary>
    public string? TeacherRemarks { get; set; }

    /// <summary>Optional guardian-facing remark for this exam (e.g. a note visible on the guardian portal).</summary>
    public string? GuardianRemarks { get; set; }
}
