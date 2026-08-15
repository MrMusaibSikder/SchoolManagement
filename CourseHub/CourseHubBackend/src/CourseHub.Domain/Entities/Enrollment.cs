using CourseHub.Domain.Common;
using CourseHub.Domain.Enums;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Domain.Entities;

/// <summary>
/// Enrollment connects a Student to a Batch. Status transitions are
/// intentionally restricted to a small, meaningful set (see Approve,
/// Complete, Cancel) — this is not a general-purpose state machine.
/// </summary>
public class Enrollment : BaseEntity
{
    public Guid InstitutionId { get; private set; }

    public Guid StudentId { get; private set; }

    public Guid BatchId { get; private set; }

    public DateTime EnrollmentDate { get; private set; }

    public EnrollmentStatus Status { get; private set; }

    private Enrollment()
    {
    }

    private Enrollment(Guid institutionId, Guid studentId, Guid batchId)
    {
        InstitutionId = institutionId;
        StudentId = studentId;
        BatchId = batchId;
        EnrollmentDate = DateTime.UtcNow;
        Status = EnrollmentStatus.Pending;
    }

    public static Enrollment Create(Guid institutionId, Guid studentId, Guid batchId)
    {
        if (institutionId == Guid.Empty)
        {
            throw new ValidationException("InstitutionId is required.");
        }

        if (studentId == Guid.Empty)
        {
            throw new ValidationException("StudentId is required.");
        }

        if (batchId == Guid.Empty)
        {
            throw new ValidationException("BatchId is required.");
        }

        return new Enrollment(institutionId, studentId, batchId);
    }

    /// <summary>
    /// Pending -> Active.
    /// </summary>
    public void Approve()
    {
        if (Status != EnrollmentStatus.Pending)
        {
            throw new DomainException($"Cannot approve an enrollment in '{Status}' status. Only 'Pending' enrollments can be approved.");
        }

        Status = EnrollmentStatus.Active;
        MarkAsUpdated();
    }

    /// <summary>
    /// Active -> Completed.
    /// </summary>
    public void Complete()
    {
        if (Status != EnrollmentStatus.Active)
        {
            throw new DomainException($"Cannot complete an enrollment in '{Status}' status. Only 'Active' enrollments can be completed.");
        }

        Status = EnrollmentStatus.Completed;
        MarkAsUpdated();
    }

    /// <summary>
    /// Pending -> Cancelled, or Active -> Cancelled.
    /// </summary>
    public void Cancel()
    {
        if (Status != EnrollmentStatus.Pending && Status != EnrollmentStatus.Active)
        {
            throw new DomainException($"Cannot cancel an enrollment in '{Status}' status.");
        }

        Status = EnrollmentStatus.Cancelled;
        MarkAsUpdated();
    }
}
