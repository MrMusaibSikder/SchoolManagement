namespace SchoolERP.Domain.Entities;

/// <summary>Join entity mapping <see cref="User"/> to <see cref="Role"/>.</summary>
public class UserRole
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
