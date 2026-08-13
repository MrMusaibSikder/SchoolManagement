using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents an academic class/grade (e.g. Class 1, Class 2).</summary>
public class SchoolClass : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public ICollection<Section> Sections { get; set; } = new List<Section>();
    public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    public ICollection<FeeStructure> FeeStructures { get; set; } = new List<FeeStructure>();
}
