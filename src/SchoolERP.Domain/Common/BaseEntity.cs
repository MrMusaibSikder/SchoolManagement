namespace SchoolERP.Domain.Common;

/// <summary>
/// Base class for all domain entities providing primary key,
/// audit fields (created/updated/deleted) and soft-delete support.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>UTC timestamp when the record was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Id of the user who created the record.</summary>
    public int? CreatedBy { get; set; }

    /// <summary>UTC timestamp when the record was last updated.</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Id of the user who last updated the record.</summary>
    public int? UpdatedBy { get; set; }

    /// <summary>Soft-delete flag. Records are never physically removed.</summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>UTC timestamp when the record was soft-deleted.</summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>Id of the user who soft-deleted the record.</summary>
    public int? DeletedBy { get; set; }
}
