using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Common.Security;

namespace CourseHub.API.Security;

/// <summary>
/// Resolves the authenticated user from the current request's claims
/// principal. Implemented here (not Infrastructure) because it depends on
/// HttpContext, a web-hosting concern owned by the API layer.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email => User?.FindFirstValue(JwtRegisteredClaimNames.Email);

    public IReadOnlyList<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray() ?? Array.Empty<string>();

    public IReadOnlyList<string> Permissions =>
        User?.FindAll(PermissionClaimTypes.Permission).Select(c => c.Value).ToArray() ?? Array.Empty<string>();
}
