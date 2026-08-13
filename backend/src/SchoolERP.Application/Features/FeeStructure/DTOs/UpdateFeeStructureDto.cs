namespace SchoolERP.Application.Features.FeeStructure.DTOs;

/// <summary>Input model for updating an existing FeeStructure record.</summary>
public class UpdateFeeStructureDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int? SectionId { get; set; }
    public bool IsTemplate { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public List<UpdateFeeStructureItemDto> Items { get; set; } = new();
}
public class UpdateFeeStructureItemDto
{
    public int? Id { get; set; } // null = new item
    public int FeeTypeId { get; set; }
    public decimal Amount { get; set; }
    public bool IsOptional { get; set; }
    public int SortOrder { get; set; }
    public bool IsDeleted { get; set; } // soft delete flag
}
