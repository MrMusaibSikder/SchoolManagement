namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Resolves the currently authenticated user from the request's claims
/// principal. Implemented in the API layer, which owns HttpContext.
/// </summary>
public interface ICurrentUserService
{
    bool IsAuthenticated { get; }

    Guid? UserId { get; }

    string? Email { get; }

    IReadOnlyList<string> Roles { get; }

    /// <summary>
    /// Effective permissions baked into the current request's JWT at
    /// issue time (see IJwtTokenService). For declarative endpoint
    /// protection prefer [HasPermission("...")]; this is for the rarer
    /// case where a controller/service needs an explicit in-code check.
    /// </summary>
    IReadOnlyList<string> Permissions { get; }
}
