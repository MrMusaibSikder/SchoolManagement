using SchoolERP.Application.Features.User.DTOs;

namespace SchoolERP.Application.Features.User.Interfaces;

/// <summary>
/// Business/service contract for User records. Services return DTOs only
/// and encapsulate all business rules for this feature, including password
/// hashing on create/update.
/// </summary>
public interface IUserService
{
    /// <summary>Retrieves every User record.</summary>
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single User record by id, or null if it does not exist.</summary>
    Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new User record. The plain-text password is hashed before persistence.</summary>
    Task<UserDto> CreateAsync(CreateUserDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing User record. If <see cref="UpdateUserDto.Password"/> is
    /// non-empty, the stored password hash is refreshed as well.
    /// </summary>
    Task<UserDto> UpdateAsync(int id, UpdateUserDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing User record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
