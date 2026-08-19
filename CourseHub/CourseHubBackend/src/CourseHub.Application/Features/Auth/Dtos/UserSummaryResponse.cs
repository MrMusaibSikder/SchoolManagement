namespace CourseHub.Application.Features.Auth.Dtos;

/// <summary>
/// Safe, public-facing projection of a User. Never includes PasswordHash.
/// Roles is a list (not a single value) because the dynamic Role system
/// (Phase 4) allows a user to hold more than one role.
/// </summary>
public record UserSummaryResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Status,
    IReadOnlyList<string> Roles);
