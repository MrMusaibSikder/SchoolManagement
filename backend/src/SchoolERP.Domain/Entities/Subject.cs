using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents an academic subject.</summary>
public class Subject : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public int FullMarks { get; set; } = 100;
    public int PassMarks { get; set; } = 33;

    public ICollection<SubjectTeacher> SubjectTeachers { get; set; } = new List<SubjectTeacher>();
    public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
}
