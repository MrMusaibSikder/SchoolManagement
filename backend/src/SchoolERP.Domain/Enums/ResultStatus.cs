namespace SchoolERP.Domain.Enums;

/// <summary>
/// Lifecycle state of an <see cref="Entities.ExamResult"/> or
/// <see cref="Entities.FinalResult"/>. Normal flow: Draft -&gt; Calculated -&gt;
/// Verified -&gt; Published -&gt; Locked -&gt; Archived. Kept alongside the existing
/// <c>IsPublished</c>/<c>PublishedAt</c>/<c>PublishedBy</c> fields (which
/// remain the source of truth for "is this visible" checks already used
/// elsewhere) purely as a richer workflow marker for reporting/auditing.
/// </summary>
public enum ResultStatus
{
    /// <summary>No calculation has been run yet.</summary>
    Draft = 1,

    /// <summary>Calculated from mark entries but not yet reviewed.</summary>
    Calculated = 2,

    /// <summary>Reviewed and confirmed correct by an authorized verifier, but not yet visible to students/guardians.</summary>
    Verified = 3,

    /// <summary>Published and visible to students/guardians.</summary>
    Published = 4,

    /// <summary>Published and additionally locked against further recalculation/edits until explicitly unlocked.</summary>
    Locked = 5,

    /// <summary>Retired from active use (e.g. superseded by a later recalculation cycle or year-end close-out).</summary>
    Archived = 6
}
