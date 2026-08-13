namespace SchoolERP.Domain.Enums;

/// <summary>
/// Lifecycle state of an <see cref="Entities.Exam"/>.
/// Draft -&gt; Published -&gt; Completed is the normal flow; an exam may instead be
/// Cancelled from Draft or Published. See <c>ExamService</c> for the exact
/// transition rules enforced for each state.
/// </summary>
public enum ExamStatus
{
    /// <summary>Exam is being prepared. Fully editable; schedules can be freely added/changed/removed.</summary>
    Draft = 1,

    /// <summary>Exam has been published. Core exam details are locked, but schedules can still be managed.</summary>
    Published = 2,

    /// <summary>Exam has finished. Fully read-only: no update, no delete, no schedule changes.</summary>
    Completed = 3,

    /// <summary>Exam has been called off. Read-only and cannot be published again.</summary>
    Cancelled = 4
}
