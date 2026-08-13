using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;

namespace SchoolERP.Infrastructure.Persistence.Seed;

public static class UserSeeder
{
    public static async Task SeedAsync(
        SchoolERPDbContext context,
        IPasswordHasher passwordHasher)
    {
        if (await context.Users.AnyAsync())
            return;

        var admin = new User
        {
            Username = "admin",
            Email = "admin@schoolerp.com",
            PasswordHash = passwordHasher.Hash("Admin@123"),
            IsActive = true
        };

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}