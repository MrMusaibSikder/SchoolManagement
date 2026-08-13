namespace SchoolERP.Domain.Entities;

/// <summary>Join entity mapping <see cref="Role"/> to <see cref="Permission"/>.</summary>
public class RolePermission
{
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}
