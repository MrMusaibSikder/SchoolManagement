using System.Net;
using System.Net.Http.Json;

namespace CourseHub.IntegrationTests;

/// <summary>
/// End-to-end tests for /api/admin/courses. See CourseHubApiFactory for
/// how these get a deterministic SuperAdmin account without depending on
/// local configuration.
/// </summary>
public class CoursesEndpointsTests : IClassFixture<CourseHubApiFactory>
{
    private readonly CourseHubApiFactory _factory;

    public CoursesEndpointsTests(CourseHubApiFactory factory)
    {
        _factory = factory;
    }

    private record CourseResponseDto(Guid Id, string Name, string Code, bool IsActive, bool IsPublic);

    [Fact]
    public async Task GetCourses_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/admin/courses");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCourses_AsStudent_ReturnsForbidden()
    {
        // A freshly-registered Student holds no admin permissions at all
        // (SeedOptions.DefaultRolePermissions has no entry for Student) —
        // this confirms the [HasPermission] gate actually blocks them,
        // not just that the endpoint exists.
        var (client, _) = await TestHelpers.CreateAuthorizedClientAsync(_factory);

        var response = await client.GetAsync("/api/admin/courses");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateCourse_DuplicateCode_ReturnsBadRequest()
    {
        var client = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        var code = $"C-{Guid.NewGuid():N}"[..12];
        var payload = new { name = "Intro to Testing", code, durationInMonths = 3, description = (string?)null };

        var first = await client.PostAsJsonAsync("/api/admin/courses", payload);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await client.PostAsJsonAsync("/api/admin/courses", payload);
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task CreateCourse_ThenDelete_SoftDeletesRatherThanRemoving()
    {
        var client = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        var code = $"C-{Guid.NewGuid():N}"[..12];

        var createResponse = await client.PostAsJsonAsync(
            "/api/admin/courses",
            new { name = "Soft Delete Test Course", code, durationInMonths = 6, description = (string?)null });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<CourseResponseDto>();
        Assert.NotNull(created);
        Assert.True(created!.IsActive);

        var deleteResponse = await client.DeleteAsync($"/api/admin/courses/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // The row must still exist and be fetchable — DELETE deactivates,
        // it never removes it (see CourseService.DeleteAsync).
        var getResponse = await client.GetAsync($"/api/admin/courses/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetched = await getResponse.Content.ReadFromJsonAsync<CourseResponseDto>();
        Assert.NotNull(fetched);
        Assert.False(fetched!.IsActive);
    }

    [Fact]
    public async Task PublishCourse_ThenAppearsInPublicCatalog()
    {
        var client = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        var code = $"C-{Guid.NewGuid():N}"[..12];

        var createResponse = await client.PostAsJsonAsync(
            "/api/admin/courses",
            new { name = "Public Catalog Test Course", code, durationInMonths = 4, description = (string?)null });
        var created = await createResponse.Content.ReadFromJsonAsync<CourseResponseDto>();

        var publishResponse = await client.PostAsync($"/api/admin/courses/{created!.Id}/publish", content: null);
        Assert.Equal(HttpStatusCode.OK, publishResponse.StatusCode);

        // Public catalog is unauthenticated — use a bare client, not the
        // SuperAdmin one, to confirm anonymous access actually works.
        var publicClient = _factory.CreateClient();
        var publicCourses = await publicClient.GetFromJsonAsync<List<CourseResponseDto>>("/api/public/courses");

        Assert.NotNull(publicCourses);
        Assert.Contains(publicCourses!, c => c.Code == code);
    }

    [Fact]
    public async Task GetCourseById_NonExistentId_ReturnsNotFound()
    {
        var client = await TestHelpers.CreateSuperAdminClientAsync(_factory);

        var response = await client.GetAsync($"/api/admin/courses/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
