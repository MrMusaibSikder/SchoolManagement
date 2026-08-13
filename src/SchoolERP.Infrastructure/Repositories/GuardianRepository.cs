using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.Guardian.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="Guardian"/> entities.
/// Works only with the <see cref="Guardian"/> entity; never returns DTOs.
/// </summary>
public class GuardianRepository : GenericRepository<Guardian>, IGuardianRepository
{
    public GuardianRepository(SchoolERPDbContext context) : base(context)
    {
    }
    public async Task<IReadOnlyList<Guardian>> SearchAsync(
    string keyword,
    CancellationToken cancellationToken = default)
    {
        keyword = keyword.Trim();

        if (string.IsNullOrWhiteSpace(keyword))
            return Array.Empty<Guardian>();

        return await DbSet
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                (x.FullName.Contains(keyword) ||
                 x.PhoneNumber.Contains(keyword)))
            .OrderBy(x => x.FullName)
            .Take(20)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> PhoneExistsAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(x =>
                !x.IsDeleted &&
                x.PhoneNumber == phoneNumber,
                cancellationToken);
    }
}

