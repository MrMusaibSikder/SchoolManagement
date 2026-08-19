namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Generates cryptographically secure, URL-safe random token strings used
/// for refresh tokens and password reset tokens. The raw value returned
/// here is what gets handed to the client/emailed to the user — it is
/// never persisted; only its hash (see ITokenHasher) is stored.
/// </summary>
public interface ISecureTokenGenerator
{
    string Generate();
}
