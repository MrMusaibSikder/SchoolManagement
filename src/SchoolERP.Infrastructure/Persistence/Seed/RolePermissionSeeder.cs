using Microsoft.EntityFrameworkCore;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;

namespace SchoolERP.Infrastructure.Persistence.Seed;

public static class RolePermissionSeeder
{
    public static async Task SeedAsync(SchoolERPDbContext context)
    {
        if (await context.RolePermissions.AnyAsync())
            return;

        var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");

        var permissions = await context.Permissions.ToListAsync();

        var rolePermissions = permissions.Select(p => new RolePermission
        {
            RoleId = adminRole.Id,
            PermissionId = p.Id
        });

        context.RolePermissions.AddRange(rolePermissions);
        await context.SaveChangesAsync();
    }
}