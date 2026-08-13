using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents a subject-wise exam date/time and marking scheme for a class.</summary>
public class ExamSchedule : BaseEntity
{
    public int ExamId { get; set; }
    public Exam? Exam { get; set; }

    public int ClassId { get; set; }
    public SchoolClass? SchoolClass { get; set; }

    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public DateTime ExamDate { get; set; }
    public int FullMarks { get; set; }
    public int PassMarks { get; set; }

    public ICollection<Result> Results { get; set; } = new List<Result>();
}
