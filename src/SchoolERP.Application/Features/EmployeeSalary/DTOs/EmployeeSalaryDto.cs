namespace SchoolERP.Application.Features.EmployeeSalary.DTOs;

/// <summary>Read model returned to clients for a EmployeeSalary record.</summary>
public class EmployeeSalaryDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public decimal BasicSalary { get; set; }
    public DateTime EffectiveFrom { get; set; }
}
