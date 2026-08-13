namespace SchoolERP.Application.Features.FeeStructure.DTOs;

/// <summary>Input model for creating a new FeeStructure record.</summary>
public class CreateFeeStructureDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int AcademicYearId { get; set; }
    public int SchoolClassId { get; set; }
    public int? SectionId { get; set; }
    public bool IsTemplate { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public int? ClonedFromId { get; set; }
    public List<CreateFeeStructureItemDto> Items { get; set; } = new();

}
public class CreateFeeStructureItemDto
{
    public int FeeTypeId { get; set; }
    public decimal Amount { get; set; }
    public bool IsOptional { get; set; }
    public int SortOrder { get; set; }
}
