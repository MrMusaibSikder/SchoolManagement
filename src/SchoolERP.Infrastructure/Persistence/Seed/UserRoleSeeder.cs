using Microsoft.EntityFrameworkCore;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;

namespace SchoolERP.Infrastructure.Persistence.Seed;

public static class UserRoleSeeder
{
    public static async Task SeedAsync(SchoolERPDbContext context)
    {
        if (await context.UserRoles.AnyAsync())
            return;

        var adminUser = await context.Users
            .FirstAsync(x => x.Username == "admin");

        var adminRole = await context.Roles
            .FirstAsync(x => x.Name == "Admin");

        context.UserRoles.Add(new UserRole
        {
            UserId = adminUser.Id,
            RoleId = adminRole.Id
        });

        await context.SaveChangesAsync();
    }
}