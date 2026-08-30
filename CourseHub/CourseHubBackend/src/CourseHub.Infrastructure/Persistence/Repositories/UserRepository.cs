using CourseHub.Application.Common.Interfaces;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CourseHubDbContext _dbContext;

    public UserRepository(CourseHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return _dbContext.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return _dbContext.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetByRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var query =
            from user in _dbContext.Users
            join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
            join role in _dbContext.Roles on userRole.RoleId equals role.Id
            where role.Name == roleName
            orderby user.FirstName, user.LastName
            select user;

        return await query.Distinct().ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<User> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim()}%";

            query = query.Where(u =>
                EF.Functions.ILike(u.FirstName, term) ||
                EF.Functions.ILike(u.LastName, term) ||
                EF.Functions.ILike(u.Email, term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }
}
