using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CourseHub.IntegrationTests;

/// <summary>
/// End-to-end tests for the auth endpoints, run against the real
/// ASP.NET Core pipeline via WebApplicationFactory. These require:
///   1. The Development connection string configured and pointing at a
///      reachable PostgreSQL database with migrations applied.
///   2. Authentication:Jwt:SecretKey set via User Secrets — without it,
///      host startup throws by design and every test here will fail at
///      WebApplicationFactory construction, not at the individual test.
///   3. Startup seeding (see DatabaseSeeder) must have run at least once
///      so the "Student"/"Teacher"/etc. roles exist — this happens
///      automatically the first time the app under test starts.
/// Each test uses a randomly generated email so tests do not collide with
/// each other or leave meaningful residue across runs.
/// </summary>
public class AuthEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_ThenLogin_ThenMe_Succeeds()
    {
        var client = _factory.CreateClient();
        var email = $"{Guid.NewGuid():N}@example.com";

        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            password = "Password123",
            confirmPassword = "Password123",
            firstName = "Jane",
            lastName = "Doe",
        });

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        var registerBody = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(registerBody);
        Assert.False(string.IsNullOrWhiteSpace(registerBody!.AccessToken));
        Assert.Contains("Student", registerBody.User.Roles);

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { email, password = "Password123" });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(loginBody);

        using var meRequest = new HttpRequestMessage(HttpMethod.Get, "/api/auth/me");
        meRequest.Headers.Add("Authorization", $"Bearer {loginBody!.AccessToken}");
        var meResponse = await client.SendAsync(meRequest);

        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
    }

    [Fact]
    public async Task Register_WithTeacherRole_AssignsTeacherRole()
    {
        var client = _factory.CreateClient();
        var email = $"{Guid.NewGuid():N}@example.com";

        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            password = "Password123",
            confirmPassword = "Password123",
            firstName = "Jane",
            lastName = "Doe",
            requestedRole = "Teacher",
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.Contains("Teacher", body!.User.Roles);
    }

    [Fact]
    public async Task Register_CannotSelfAssignAdminRole()
    {
        var client = _factory.CreateClient();
        var email = $"{Guid.NewGuid():N}@example.com";

        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            password = "Password123",
            confirmPassword = "Password123",
            firstName = "Jane",
            lastName = "Doe",
            requestedRole = "Admin",
        });

        // Validation rejects "Admin" as a self-selectable RequestedRole —
        // only Teacher/Student are allowed via the public endpoint.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Me_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var email = $"{Guid.NewGuid():N}@example.com";

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            password = "Password123",
            confirmPassword = "Password123",
            firstName = "Jane",
            lastName = "Doe",
        });

        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password = "WrongPassword" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();
        var email = $"{Guid.NewGuid():N}@example.com";

        var payload = new
        {
            email,
            password = "Password123",
            confirmPassword = "Password123",
            firstName = "Jane",
            lastName = "Doe",
        };

        var first = await client.PostAsJsonAsync("/api/auth/register", payload);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await client.PostAsJsonAsync("/api/auth/register", payload);
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task PublicInstitution_ReturnsSeededLandingPageData()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/public/institution");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private record UserSummaryDto(Guid Id, string Email, string FirstName, string LastName, string Status, string[] Roles);

    private record AuthResponseDto(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc, UserSummaryDto User);
}
