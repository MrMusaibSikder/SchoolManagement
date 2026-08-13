namespace SchoolERP.Application.Features.EmployeeSalary.DTOs;

/// <summary>Input model for creating a new EmployeeSalary record.</summary>
public class CreateEmployeeSalaryDto
{
    public int EmployeeId { get; set; }
    public decimal BasicSalary { get; set; }
    public DateTime EffectiveFrom { get; set; }
}
