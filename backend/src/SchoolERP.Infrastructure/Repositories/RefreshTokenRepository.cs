using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.RefreshToken.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="RefreshToken"/> entities.
/// Works only with the entity; never returns DTOs.
/// </summary>
public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    private readonly SchoolERPDbContext _context;

    public RefreshTokenRepository(SchoolERPDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(x => x.Token == token && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<RefreshToken>()
            .Where(x => !x.IsDeleted &&
                        x.UserId == userId &&
                        x.RevokedAt == null &&
                        x.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }
}
