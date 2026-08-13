namespace SchoolERP.Application.Common.Interfaces.Services;

/// <summary>
/// Hashes and verifies user passwords. Kept as an abstraction so the hashing
/// algorithm can be swapped without touching business logic in services.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Produces a salted hash for the given plain-text password.</summary>
    string Hash(string password);

    /// <summary>Verifies a plain-text password against a previously generated hash.</summary>
    bool Verify(string password, string passwordHash);
}
