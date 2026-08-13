using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents teaching-specific details linked one-to-one to an <see cref="Employee"/>.</summary>
public class Teacher : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public string? Qualification { get; set; }
    public string? Specialization { get; set; }

    public ICollection<SubjectTeacher> SubjectTeachers { get; set; } = new List<SubjectTeacher>();
}
