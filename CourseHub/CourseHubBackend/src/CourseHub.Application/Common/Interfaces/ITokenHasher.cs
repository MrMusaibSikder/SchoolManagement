namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// One-way hash for opaque tokens (refresh tokens, password reset tokens)
/// before they are persisted or looked up. Not a password hasher — this is
/// for high-entropy random tokens, so a fast, deterministic hash (SHA-256)
/// is appropriate and lets lookups use a simple equality/index match.
/// </summary>
public interface ITokenHasher
{
    string Hash(string rawToken);
}
