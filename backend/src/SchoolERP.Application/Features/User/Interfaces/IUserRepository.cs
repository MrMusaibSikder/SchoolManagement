using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.User.Interfaces;

/// <summary>
/// Repository contract for <see cref="User"/> entities.
/// Extends the generic repository with an User-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IUserRepository : IGenericRepository<SchoolERP.Domain.Entities.User>
{
    Task<SchoolERP.Domain.Entities.User?> GetByUsernameAsync(string username);

    Task<SchoolERP.Domain.Entities.User?> GetByEmailAsync(string email);

    Task<SchoolERP.Domain.Entities.User?> GetByUsernameOrEmailAsync(string usernameOrEmail);

    Task<bool> UsernameExistsAsync(string username);

    Task<bool> EmailExistsAsync(string email);
}
