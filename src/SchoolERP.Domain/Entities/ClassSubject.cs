namespace SchoolERP.Domain.Entities;

/// <summary>Join entity mapping <see cref="SchoolClass"/> to <see cref="Subject"/>.</summary>
public class ClassSubject
{
    public int ClassId { get; set; }
    public SchoolClass SchoolClass { get; set; } = null!;

    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    /// <summary>
    /// Whether this subject is optional (e.g. Higher Math, Agriculture, ICT
    /// Practical) for this class. Optional subjects never count toward a
    /// student's fail count and contribute only a bonus grade point toward
    /// the final GPA (per the standard Bangladesh "4th subject" rule); see
    /// <c>ExamResultService</c>/<c>FinalResultService</c> for the calculation.
    /// </summary>
    public bool IsOptional { get; set; }
}
