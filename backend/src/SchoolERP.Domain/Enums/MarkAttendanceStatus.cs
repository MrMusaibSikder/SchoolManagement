namespace SchoolERP.Domain.Enums;

/// <summary>
/// Attendance state of a student for a specific subject's mark entry
/// (distinct from day-to-day <see cref="AttendanceStatus"/>). Determines
/// whether <see cref="Entities.Result.MarksObtained"/> is meaningful.
/// </summary>
public enum MarkAttendanceStatus
{
    /// <summary>Student sat the exam; marks are entered normally.</summary>
    Present = 1,

    /// <summary>Student did not sit the exam. Marks must be 0.</summary>
    Absent = 2,

    /// <summary>Student was excused for a medical reason. Marks must be 0; typically excluded from pass/fail averaging by policy.</summary>
    Medical = 3,

    /// <summary>Result withheld (e.g. disciplinary/administrative hold). Marks must be 0.</summary>
    Withheld = 4,

    /// <summary>Only some components of the exam were completed. Marks reflect only what was assessed.</summary>
    Incomplete = 5,

    /// <summary>Student was excused from sitting the exam (non-medical, e.g. approved leave). Marks must be 0.</summary>
    Excused = 6,

    /// <summary>Student arrived late. Marks are entered normally; flagged for record-keeping only.</summary>
    Late = 7,

    /// <summary>Student was caught cheating. Always graded as a fail (F / GPA 0), regardless of marks.</summary>
    Cheating = 8,

    /// <summary>Entry is excluded from result calculation entirely (e.g. pending investigation). Not counted toward totals, averages, or pass/fail.</summary>
    Blocked = 9
}
