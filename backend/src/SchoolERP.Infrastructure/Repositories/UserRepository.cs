using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.User.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="User"/> entities.
/// Works only with the <see cref="User"/> entity; never returns DTOs.
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly SchoolERPDbContext _context;

    public UserRepository(SchoolERPDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x =>
                !x.IsDeleted &&
                x.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x =>
                !x.IsDeleted &&
                x.Email == email);
    }

    public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x =>
                !x.IsDeleted &&
                (x.Username == usernameOrEmail ||
                 x.Email == usernameOrEmail));
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users.AnyAsync(x =>
            !x.IsDeleted &&
            x.Username == username);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(x =>
            !x.IsDeleted &&
            x.Email == email);
    }
}
