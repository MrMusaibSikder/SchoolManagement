namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Abstraction over password hashing/verification. Implemented in
/// Infrastructure using ASP.NET Core Identity's PasswordHasher primitives
/// (PBKDF2) — never a custom or reversible scheme.
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);

    /// <summary>
    /// Returns true if providedPassword matches the given hash.
    /// </summary>
    bool VerifyPassword(string passwordHash, string providedPassword);
}
