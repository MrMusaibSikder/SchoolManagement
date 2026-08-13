using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents a section within a <see cref="SchoolClass"/> (e.g. A, B, C).</summary>
public class Section : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int ClassId { get; set; }
    public SchoolClass SchoolClass { get; set; } = null!;

    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<FeeStructure> FeeStructures { get; set; } = new List<FeeStructure>();
}
