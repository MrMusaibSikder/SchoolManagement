using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

/// <summary>
/// The overall, weighted result for a student across an entire academic year,
/// computed from every <see cref="ExamResult"/> for that student using the
/// active <see cref="ExamWeightSetup"/>. One row per Student + AcademicYear.
/// </summary>
public class FinalResult : BaseEntity
{
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public int AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    public int ExamWeightSetupId { get; set; }
    public ExamWeightSetup? ExamWeightSetup { get; set; }

    /// <summary>Sum of every subject's weighted final marks (see <see cref="FinalResultDetail.FinalMarks"/>).</summary>
    public decimal FinalMarks { get; set; }

    /// <summary>Average grade point across every subject's weighted result.</summary>
    public decimal FinalGPA { get; set; }

    /// <summary>Overall letter grade derived from <see cref="FinalGPA"/>.</summary>
    public string FinalGrade { get; set; } = string.Empty;

    /// <summary>False if the student failed any single subject in the weighted result, or the overall average is below the pass threshold.</summary>
    public bool IsPassed { get; set; }

    /// <summary>Promotion outcome for the next academic year/class.</summary>
    public PromotionStatus PromotionStatus { get; set; } = PromotionStatus.Pending;

    /// <summary>1-based rank school-wide (or year-wide) by FinalGPA. Null until calculated.</summary>
    public int? MeritPosition { get; set; }

    /// <summary>1-based rank within the student's class. Null until calculated.</summary>
    public int? ClassPosition { get; set; }

    /// <summary>1-based rank within the student's section. Null until calculated.</summary>
    public int? SectionPosition { get; set; }

    /// <summary>Whether this final result has been published (visible to students/guardians) and locked.</summary>
    public bool IsPublished { get; set; }

    /// <summary>UTC timestamp the final result was published, if any.</summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>Id of the User who published this final result.</summary>
    public int? PublishedBy { get; set; }

    public ICollection<FinalResultDetail> Details { get; set; } = new List<FinalResultDetail>();

    /// <summary>Optional class/subject teacher's remark for the year, shown on the transcript.</summary>
    public string? TeacherRemarks { get; set; }

    /// <summary>Optional principal/head-teacher remark for the year, shown on the transcript.</summary>
    public string? PrincipalRemarks { get; set; }
}
