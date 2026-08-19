using CourseHub.Domain.Entities;

namespace CourseHub.Application.Common.Interfaces;

public record AccessTokenResult(string Token, DateTime ExpiresAtUtc);

/// <summary>
/// Issues signed JWT access tokens for an authenticated User.
/// Implemented in Infrastructure using Microsoft.IdentityModel.Tokens.
/// Roles/permissions are passed in explicitly (not read from User)
/// because that assignment lives in separate tables, not on User itself —
/// see the Phase 4 dynamic Role/Permission design.
///
/// Permissions are resolved from the user's roles and baked into the
/// token at issue time (Phase 9), rather than looked up from the database
/// on every request. This means a role's permissions change takes effect
/// on the user's next login/refresh, not instantly — an accepted
/// trade-off given access tokens are short-lived and refresh is cheap.
/// </summary>
public interface IJwtTokenService
{
    AccessTokenResult GenerateAccessToken(User user, IReadOnlyList<string> roles, IReadOnlyList<string> permissions);
}
