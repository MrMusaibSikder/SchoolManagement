using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

/// <summary>
/// Represents a student's mark entry and computed subject result for a
/// specific exam schedule (subject). This is the "Mark Entry" record: a
/// teacher enters <see cref="MarksObtained"/> (plus attendance/grace/remarks)
/// for their assigned class/subject/exam, and the subject-level
/// Grade/GPA/Pass-Fail are computed from it. <see cref="ExamResult"/>
/// aggregates these rows across every subject of one exam for one student;
/// <see cref="FinalResultDetail"/> aggregates them (weighted) across exams.
/// </summary>
public class Result : BaseEntity
{
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public int ExamScheduleId { get; set; }
    public ExamSchedule? ExamSchedule { get; set; }

    /// <summary>Raw marks entered by the teacher (before grace marks). 0 when <see cref="AttendanceStatus"/> is not Present.</summary>
    public decimal MarksObtained { get; set; }

    /// <summary>Extra marks awarded on top of <see cref="MarksObtained"/> (e.g. board-approved grace). Included in grading.</summary>
    public decimal GraceMarks { get; set; }

    /// <summary>Computed letter grade (e.g. "A+"), based on (MarksObtained + GraceMarks) against the exam schedule's FullMarks/PassMarks.</summary>
    public string? Grade { get; set; }

    /// <summary>Computed grade point (e.g. 5.00 for A+), matching <see cref="Grade"/>.</summary>
    public decimal? GPA { get; set; }

    /// <summary>Computed pass/fail outcome for this subject.</summary>
    public bool IsPassed { get; set; }

    /// <summary>Computed percentage of (MarksObtained + GraceMarks) over FullMarks.</summary>
    public decimal? Percentage { get; set; }

    /// <summary>Student's exam-attendance state for this subject. Determines whether marks are meaningful.</summary>
    public MarkAttendanceStatus AttendanceStatus { get; set; } = MarkAttendanceStatus.Present;

    /// <summary>Draft/Submitted workflow state of this mark entry.</summary>
    public MarkEntryStatus EntryStatus { get; set; } = MarkEntryStatus.Draft;

    /// <summary>Optional teacher remark for this specific mark entry.</summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// True once the parent exam's <see cref="ExamResult"/> has been published
    /// (or an admin has explicitly locked it). Locked marks cannot be edited
    /// until an admin unlocks them.
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>UTC timestamp the entry was locked, if any.</summary>
    public DateTime? LockedAt { get; set; }

    /// <summary>Id of the Teacher who entered/last edited this mark (assignment-checked at entry time).</summary>
    public int? EnteredByTeacherId { get; set; }
    public Teacher? EnteredByTeacher { get; set; }
}
