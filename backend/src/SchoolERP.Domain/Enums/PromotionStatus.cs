namespace SchoolERP.Domain.Enums;

/// <summary>Promotion outcome for a student's <see cref="Entities.FinalResult"/> in an academic year.</summary>
public enum PromotionStatus
{
    /// <summary>Final result not yet calculated/published.</summary>
    Pending = 1,

    /// <summary>Student met the promotion criteria for the next academic year/class.</summary>
    Promoted = 2,

    /// <summary>Student did not meet the promotion criteria.</summary>
    NotPromoted = 3
}
