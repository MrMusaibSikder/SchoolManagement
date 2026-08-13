using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents a staff member (teaching or non-teaching).</summary>
public class Employee : BaseEntity
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime JoiningDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string? EmployeePhoto {  get; set; }

    public int DesignationId { get; set; }
    public Designation Designation { get; set; } = null!;

    public int? UserId { get; set; }
    public User? User { get; set; }

    public Teacher? Teacher { get; set; }

    public ICollection<EmployeeAttendance> Attendances { get; set; } = new List<EmployeeAttendance>();
    public ICollection<EmployeeSalary> Salaries { get; set; } = new List<EmployeeSalary>();
}
