using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents a job title/designation assigned to employees.</summary>
public class Designation : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
