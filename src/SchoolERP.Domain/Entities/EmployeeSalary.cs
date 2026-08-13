using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents a salary record effective from a given date for an employee.</summary>
public class EmployeeSalary : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public decimal BasicSalary { get; set; }
    public DateTime EffectiveFrom { get; set; }
}
