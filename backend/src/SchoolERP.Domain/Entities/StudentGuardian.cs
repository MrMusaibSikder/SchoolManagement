namespace SchoolERP.Domain.Entities;

/// <summary>Join entity mapping <see cref="Student"/> to <see cref="Guardian"/> with a relation type.</summary>
public class StudentGuardian
{
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public int GuardianId { get; set; }
    public Guardian? Guardian { get; set; }

    public string Relation { get; set; } = string.Empty;
}
