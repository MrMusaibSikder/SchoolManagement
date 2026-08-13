using SchoolERP.Domain.Common;
namespace SchoolERP.Domain.Entities;

/// <summary>Represents an enrolled student.</summary>
public class Student : BaseEntity
{
    public string AdmissionNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string RollNo { get; set; } = string.Empty;
    public DateTime AdmissionDate { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? BloodGroup { get; set; }
    public string? Address { get; set; }
    public string? Photo { get; set; }

    public int ClassId { get; set; }
    public SchoolClass SchoolClass { get; set; } = null!;

    public int? SectionId { get; set; }
    public Section? Section { get; set; } = null;

    public ICollection<StudentGuardian> StudentGuardians { get; set; } = new List<StudentGuardian>();
    public ICollection<StudentAttendance> Attendances { get; set; } = new List<StudentAttendance>();
    public ICollection<Result> Results { get; set; } = new List<Result>();
    public ICollection<StudentFeeConcession> FeeConcessions { get; set; } = new List<StudentFeeConcession>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
