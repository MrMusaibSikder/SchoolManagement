using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.PasswordResetToken.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="PasswordResetToken"/> entities.
/// Works only with the entity; never returns DTOs.
/// </summary>
public class PasswordResetTokenRepository : GenericRepository<PasswordResetToken>, IPasswordResetTokenRepository
{
    private readonly SchoolERPDbContext _context;

    public PasswordResetTokenRepository(SchoolERPDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.Set<PasswordResetToken>()
            .FirstOrDefaultAsync(x => x.Token == token && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<PasswordResetToken>> GetActiveByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<PasswordResetToken>()
            .Where(x => !x.IsDeleted &&
                        x.UserId == userId &&
                        !x.IsUsed &&
                        x.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }
}
