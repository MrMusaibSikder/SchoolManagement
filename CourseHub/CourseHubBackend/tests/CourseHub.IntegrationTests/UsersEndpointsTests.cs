using System.Net;
using System.Net.Http.Json;

namespace CourseHub.IntegrationTests;

public class UsersEndpointsTests : IClassFixture<CourseHubApiFactory>
{
    private readonly CourseHubApiFactory _factory;

    public UsersEndpointsTests(CourseHubApiFactory factory)
    {
        _factory = factory;
    }

    private record UserResponseDto(Guid Id, string Email, string Status, string[] Roles);

    [Fact]
    public async Task AssignRole_SuperAdmin_ReturnsBadRequest()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        var (_, targetAuth) = await TestHelpers.CreateAuthorizedClientAsync(_factory);

        var response = await superAdmin.PostAsJsonAsync($"/api/admin/users/{targetAuth.User.Id}/roles", new { roleName = "SuperAdmin" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AssignRole_ThenRemoveRole_BothIdempotent()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        // Registers as Student — will additionally get Teacher assigned below.
        var (_, targetAuth) = await TestHelpers.CreateAuthorizedClientAsync(_factory);

        var assignResponse = await superAdmin.PostAsJsonAsync($"/api/admin/users/{targetAuth.User.Id}/roles", new { roleName = "Teacher" });
        Assert.Equal(HttpStatusCode.OK, assignResponse.StatusCode);
        var afterAssign = await assignResponse.Content.ReadFromJsonAsync<UserResponseDto>();
        Assert.Contains("Teacher", afterAssign!.Roles);
        Assert.Contains("Student", afterAssign.Roles);

        // Assigning it again must not error or duplicate.
        var reassignResponse = await superAdmin.PostAsJsonAsync($"/api/admin/users/{targetAuth.User.Id}/roles", new { roleName = "Teacher" });
        Assert.Equal(HttpStatusCode.OK, reassignResponse.StatusCode);

        var removeResponse = await superAdmin.DeleteAsync($"/api/admin/users/{targetAuth.User.Id}/roles/Teacher");
        Assert.Equal(HttpStatusCode.OK, removeResponse.StatusCode);
        var afterRemove = await removeResponse.Content.ReadFromJsonAsync<UserResponseDto>();
        Assert.DoesNotContain("Teacher", afterRemove!.Roles);

        // Removing a role the user no longer has must still succeed.
        var reremoveResponse = await superAdmin.DeleteAsync($"/api/admin/users/{targetAuth.User.Id}/roles/Teacher");
        Assert.Equal(HttpStatusCode.OK, reremoveResponse.StatusCode);
    }

    [Fact]
    public async Task Suspend_BlocksSubsequentLogin()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        var email = $"{Guid.NewGuid():N}@example.com";
        var client = _factory.CreateClient();
        var targetAuth = await TestHelpers.RegisterAsync(client, email: email);

        var suspendResponse = await superAdmin.PostAsync($"/api/admin/users/{targetAuth.User.Id}/suspend", content: null);
        Assert.Equal(HttpStatusCode.OK, suspendResponse.StatusCode);
        var suspended = await suspendResponse.Content.ReadFromJsonAsync<UserResponseDto>();
        Assert.Equal("Suspended", suspended!.Status);

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { email, password = "Password123" });
        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
    }

    [Fact]
    public async Task SearchUsers_AsSuperAdmin_FindsSeededResults()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);

        var response = await superAdmin.GetAsync("/api/admin/users?pageSize=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<UserResponseDto>>();
        Assert.NotNull(result);
        Assert.True(result!.TotalCount >= 1);
    }
}
