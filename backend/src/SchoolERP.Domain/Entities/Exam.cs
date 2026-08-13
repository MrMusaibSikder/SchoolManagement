using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents a scheduled examination event for an academic year.</summary>
public class Exam : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int ExamTypeId { get; set; }
    public ExamType? ExamType { get; set; }

    public int AcademicYearId { get; set; }
    public AcademicYear AcademicYear { get; set; } = null!;

    /// <summary>Current lifecycle state of the exam. See <see cref="ExamStatus"/> for the allowed transitions.</summary>
    public ExamStatus Status { get; set; } = ExamStatus.Draft;

    public ICollection<ExamSchedule> ExamSchedules { get; set; } = new List<ExamSchedule>();
}
