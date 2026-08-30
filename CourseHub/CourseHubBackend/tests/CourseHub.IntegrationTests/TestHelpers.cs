using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CourseHub.IntegrationTests;

public record UserSummaryDto(Guid Id, string Email, string FirstName, string LastName, string Status, string[] Roles);

public record AuthResponseDto(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc, UserSummaryDto User);

public record PagedResultDto<T>(List<T> Items, int TotalCount, int Page, int PageSize, int TotalPages);

/// <summary>
/// Shared setup for integration tests: registering a user (with an
/// optional role/SuperAdmin invite code) and attaching a Bearer token to
/// an HttpClient. Centralized so every *EndpointsTests.cs file doesn't
/// re-implement the same "register, then extract the token" boilerplate.
/// </summary>
internal static class TestHelpers
{
    public static async Task<AuthResponseDto> RegisterAsync(
        HttpClient client,
        string? requestedRole = null,
        string? superAdminCode = null,
        string? email = null)
    {
        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = email ?? $"{Guid.NewGuid():N}@example.com",
            password = "Password123",
            confirmPassword = "Password123",
            firstName = "Test",
            lastName = "User",
            requestedRole,
            superAdminCode,
        });

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return body ?? throw new InvalidOperationException("Register response body was null.");
    }

    public static void AuthorizeWith(this HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    /// <summary>
    /// Registers a brand-new SuperAdmin (via the fixed invite code
    /// CourseHubApiFactory always seeds) and returns a client already
    /// carrying its Bearer token — for tests that need to exercise
    /// permission-gated admin endpoints end-to-end without depending on
    /// any pre-existing account.
    /// </summary>
    public static async Task<HttpClient> CreateSuperAdminClientAsync(CourseHubApiFactory factory)
    {
        var client = factory.CreateClient();
        var auth = await RegisterAsync(client, superAdminCode: CourseHubApiFactory.SuperAdminInviteCode);
        client.AuthorizeWith(auth.AccessToken);
        return client;
    }

    public static async Task<(HttpClient Client, AuthResponseDto Auth)> CreateAuthorizedClientAsync(
        WebApplicationFactory<Program> factory,
        string? requestedRole = null)
    {
        var client = factory.CreateClient();
        var auth = await RegisterAsync(client, requestedRole);
        client.AuthorizeWith(auth.AccessToken);
        return (client, auth);
    }
}
