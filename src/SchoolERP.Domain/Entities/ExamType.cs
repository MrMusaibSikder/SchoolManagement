using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents a category of exam (e.g. Term, Half-Yearly, Final).</summary>
public class ExamType : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
