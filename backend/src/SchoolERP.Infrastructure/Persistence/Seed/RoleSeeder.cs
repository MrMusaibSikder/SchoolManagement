using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace SchoolERP.Infrastructure.Persistence.Seed;

public static class RoleSeeder
{
    public static async Task SeedAsync(SchoolERPDbContext context)
    {
        if (await context.Roles.AnyAsync())
            return;

        var roles = new List<Role>
        {
            new Role { Name = "Admin", Description = "System Administrator" },
            new Role { Name = "Teacher", Description = "Teacher Role" },
            new Role { Name = "Student", Description = "Student Role" },
            new Role { Name = "Accountant", Description = "Fee Management Role" }
        };

        context.Roles.AddRange(roles);
        await context.SaveChangesAsync();
    }
}