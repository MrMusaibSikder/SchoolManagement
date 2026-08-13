namespace SchoolERP.Application.Features.FeeStructure.DTOs;

/// <summary>Read model returned to clients for a FeeStructure record.</summary>
public class FeeStructureDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int AcademicYearId { get; set; }
    public string? AcademicYearName { get; set; }
    public int SchoolClassId { get; set; }
    public string? SchoolClassName { get; set; }
    public int? SectionId { get; set; }
    public string? SectionName { get; set; }
    public bool IsActive { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public List<FeeStructureItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
public class FeeStructureItemDto
{
    public int Id { get; set; }
    public int FeeTypeId { get; set; }
    public string? FeeTypeName { get; set; }
    public string? FeeTypeCode { get; set; }
    public decimal Amount { get; set; }
    public bool IsOptional { get; set; }
    public int SortOrder { get; set; }
}
