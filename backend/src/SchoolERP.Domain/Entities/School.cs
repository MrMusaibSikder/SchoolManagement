using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents core school/institution profile information.</summary>
public class School : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? EIIN { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Logo { get; set; }
}
