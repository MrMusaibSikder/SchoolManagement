namespace SchoolERP.Application.Features.UserRole.DTOs;

/// <summary>Input model for the "Assign Role to User" admin action.</summary>
public class AssignRoleToUserDto
{
    public int UserId { get; set; }

    public int RoleId { get; set; }
}
