namespace SchoolERP.Domain.Enums;

/// <summary>
/// Workflow state of a single <see cref="Entities.Result"/> (mark entry) row,
/// independent of <see cref="Entities.Result.IsLocked"/>.
/// </summary>
public enum MarkEntryStatus
{
    /// <summary>Saved but not yet finalized by the teacher. Freely editable.</summary>
    Draft = 1,

    /// <summary>Finalized by the teacher and ready for result calculation/publish.</summary>
    Submitted = 2
}
