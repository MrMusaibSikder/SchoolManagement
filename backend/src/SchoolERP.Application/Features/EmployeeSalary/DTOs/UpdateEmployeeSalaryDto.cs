namespace SchoolERP.Application.Features.EmployeeSalary.DTOs;

/// <summary>Input model for updating an existing EmployeeSalary record.</summary>
public class UpdateEmployeeSalaryDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public decimal BasicSalary { get; set; }
    public DateTime EffectiveFrom { get; set; }
}
