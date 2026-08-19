using CourseHub.Application.Common.Interfaces;
using CourseHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CourseHub.Infrastructure.Authentication;

/// <summary>
/// Password hashing via ASP.NET Core Identity's PasswordHasher&lt;TUser&gt;
/// primitive (PBKDF2, salted, versioned format) — not the full Identity
/// membership system, just its battle-tested hashing algorithm. User is
/// used only as PasswordHasher's generic type parameter; none of its
/// members are read by the hasher itself.
/// </summary>
public class PasswordHasherAdapter : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string HashPassword(string password)
    {
        return _hasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string passwordHash, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(null!, passwordHash, providedPassword);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
