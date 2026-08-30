using System.Net;
using System.Net.Http.Json;

namespace CourseHub.IntegrationTests;

public class StudentsEndpointsTests : IClassFixture<CourseHubApiFactory>
{
    private readonly CourseHubApiFactory _factory;

    public StudentsEndpointsTests(CourseHubApiFactory factory)
    {
        _factory = factory;
    }

    private record StudentResponseDto(Guid Id, Guid UserId, string StudentId, string FirstName, string LastName, bool IsActive);

    [Fact]
    public async Task CreateStudent_ForUserWithoutStudentRole_ReturnsBadRequest()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        var (_, teacherAuth) = await TestHelpers.CreateAuthorizedClientAsync(_factory, requestedRole: "Teacher");

        var response = await superAdmin.PostAsJsonAsync("/api/admin/students", new
        {
            userId = teacherAuth.User.Id,
            studentId = $"STU-{Guid.NewGuid():N}"[..12],
            firstName = "Karim",
            lastName = "Islam",
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateStudent_HappyPath_ThenDeactivate_StillFetchable()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        // Student is the default role on registration.
        var (_, studentAuth) = await TestHelpers.CreateAuthorizedClientAsync(_factory);

        var createResponse = await superAdmin.PostAsJsonAsync("/api/admin/students", new
        {
            userId = studentAuth.User.Id,
            studentId = $"STU-{Guid.NewGuid():N}"[..12],
            firstName = "Karim",
            lastName = "Islam",
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<StudentResponseDto>();
        Assert.NotNull(created);
        Assert.True(created!.IsActive);

        var deactivateResponse = await superAdmin.PostAsync($"/api/admin/students/{created.Id}/deactivate", content: null);
        Assert.Equal(HttpStatusCode.OK, deactivateResponse.StatusCode);
        var deactivated = await deactivateResponse.Content.ReadFromJsonAsync<StudentResponseDto>();
        Assert.False(deactivated!.IsActive);
    }

    [Fact]
    public async Task GetStudents_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/admin/students");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
