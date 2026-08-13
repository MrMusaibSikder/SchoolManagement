namespace SchoolERP.Domain.Entities;

/// <summary>Join entity mapping <see cref="Subject"/> to <see cref="Teacher"/>.</summary>
public class SubjectTeacher
{
    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
}
