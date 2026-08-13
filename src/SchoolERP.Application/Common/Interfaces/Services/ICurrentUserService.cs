namespace SchoolERP.Application.Common.Interfaces.Services;

/// <summary>
/// Exposes information about the user making the current HTTP request, derived
/// from the validated JWT claims. Implemented in the Api layer (it depends on
/// <c>IHttpContextAccessor</c>) so Application/Infrastructure services can stay
/// framework-agnostic while still knowing "who is calling".
/// </summary>
public interface ICurrentUserService
{
    /// <summary>The authenticated user's id, or null if there is no authenticated user.</summary>
    int? UserId { get; }

    /// <summary>The authenticated user's username, or null if unavailable.</summary>
    string? Username { get; }

    /// <summary>The authenticated user's email, or null if unavailable.</summary>
    string? Email { get; }

    /// <summary>The role names carried by the current JWT.</summary>
    IReadOnlyList<string> Roles { get; }

    /// <summary>Whether the current request has a valid authenticated user.</summary>
    bool IsAuthenticated { get; }
}
