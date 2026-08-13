using SchoolERP.Domain.Constants;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace SchoolERP.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds every permission declared as a constant on <see cref="PermissionNames"/>.
/// Using <see cref="PermissionNames.GetAll"/> as the single source of truth means
/// new permission groups only ever need to be added in one place.
/// </summary>
public static class PermissionSeeder
{
    public static async Task SeedAsync(SchoolERPDbContext context)
    {
        if (await context.Permissions.AnyAsync())
            return;

        var permissions = PermissionNames.GetAll()
            .Select(name => new Permission { Name = name })
            .ToList();

        context.Permissions.AddRange(permissions);
        await context.SaveChangesAsync();
    }
}