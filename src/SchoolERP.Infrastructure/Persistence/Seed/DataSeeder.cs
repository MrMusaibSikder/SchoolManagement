using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Infrastructure.Persistence.Context;

namespace SchoolERP.Infrastructure.Persistence.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await context.Database.MigrateAsync();

        await RoleSeeder.SeedAsync(context);

        await PermissionSeeder.SeedAsync(context);

        await RolePermissionSeeder.SeedAsync(context);

        await UserSeeder.SeedAsync(context, passwordHasher);

        await UserRoleSeeder.SeedAsync(context);

        await FeeManagementSeeder.SeedAsync(context);
    }
}