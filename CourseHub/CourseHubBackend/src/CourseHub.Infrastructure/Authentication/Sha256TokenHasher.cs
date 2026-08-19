using System.Security.Cryptography;
using System.Text;
using CourseHub.Application.Common.Interfaces;

namespace CourseHub.Infrastructure.Authentication;

/// <summary>
/// SHA-256 hash for high-entropy opaque tokens (refresh/reset tokens).
/// This is intentionally NOT used for passwords — passwords use
/// PasswordHasherAdapter (PBKDF2, salted, slow-by-design). A fast
/// deterministic hash is correct here because the input is already a
/// 64-byte random value, not something guessable via brute force, and a
/// deterministic hash is what allows an equality-indexed database lookup.
/// </summary>
public class Sha256TokenHasher : ITokenHasher
{
    public string Hash(string rawToken)
    {
        var bytes = Encoding.UTF8.GetBytes(rawToken);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
