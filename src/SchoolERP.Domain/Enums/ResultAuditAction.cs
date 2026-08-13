namespace SchoolERP.Domain.Enums;

/// <summary>The kind of action recorded by a <see cref="Entities.ResultAuditLog"/> entry.</summary>
public enum ResultAuditAction
{
    Calculated = 1,
    Recalculated = 2,
    Verified = 3,
    Published = 4,
    Unpublished = 5,
    Locked = 6,
    Unlocked = 7,
    Archived = 8,
    RolledBack = 9,
    MarkUpdated = 10
}
