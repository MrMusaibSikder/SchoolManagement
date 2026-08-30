using System.Net;
using System.Net.Http.Json;

namespace CourseHub.IntegrationTests;

/// <summary>
/// End-to-end tests for /api/admin/teachers, focused on the "promote an
/// existing User" pattern that's unique to this controller.
/// </summary>
public class TeachersEndpointsTests : IClassFixture<CourseHubApiFactory>
{
    private readonly CourseHubApiFactory _factory;

    public TeachersEndpointsTests(CourseHubApiFactory factory)
    {
        _factory = factory;
    }

    private record TeacherResponseDto(Guid Id, Guid UserId, string EmployeeId, string FirstName, string LastName, bool IsActive);

    private record EligibleUserDto(Guid Id, string Email, string FirstName, string LastName);

    [Fact]
    public async Task CreateTeacher_ForUserWithoutTeacherRole_ReturnsBadRequest()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        // Default role is Student, not Teacher.
        var (_, studentAuth) = await TestHelpers.CreateAuthorizedClientAsync(_factory);

        var response = await superAdmin.PostAsJsonAsync("/api/admin/teachers", new
        {
            userId = studentAuth.User.Id,
            employeeId = $"EMP-{Guid.NewGuid():N}"[..12],
            firstName = "Rahim",
            lastName = "Uddin",
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTeacher_HappyPath_UserDisappearsFromEligibleList()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        var (_, teacherAuth) = await TestHelpers.CreateAuthorizedClientAsync(_factory, requestedRole: "Teacher");

        var eligibleBefore = await superAdmin.GetFromJsonAsync<List<EligibleUserDto>>("/api/admin/teachers/eligible-users");
        Assert.NotNull(eligibleBefore);
        Assert.Contains(eligibleBefore!, u => u.Id == teacherAuth.User.Id);

        var employeeId = $"EMP-{Guid.NewGuid():N}"[..12];
        var createResponse = await superAdmin.PostAsJsonAsync("/api/admin/teachers", new
        {
            userId = teacherAuth.User.Id,
            employeeId,
            firstName = "Rahim",
            lastName = "Uddin",
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<TeacherResponseDto>();
        Assert.NotNull(created);
        Assert.Equal(teacherAuth.User.Id, created!.UserId);
        Assert.True(created.IsActive);

        // A second promotion attempt for the same user must now fail —
        // one profile per user (unique UserId, enforced before the DB
        // constraint even gets a chance to reject it).
        var duplicateResponse = await superAdmin.PostAsJsonAsync("/api/admin/teachers", new
        {
            userId = teacherAuth.User.Id,
            employeeId = $"EMP-{Guid.NewGuid():N}"[..12],
            firstName = "Rahim",
            lastName = "Uddin",
        });
        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);

        var eligibleAfter = await superAdmin.GetFromJsonAsync<List<EligibleUserDto>>("/api/admin/teachers/eligible-users");
        Assert.DoesNotContain(eligibleAfter!, u => u.Id == teacherAuth.User.Id);
    }

    [Fact]
    public async Task CreateTeacher_AsTeacherWithoutCreatePermission_ReturnsForbidden()
    {
        // Teacher's default permission set (courses.view, batches.view,
        // enrollments.view, enrollments.update) does NOT include
        // teachers.create — creating other teacher profiles is
        // admin-only by default.
        var (client, _) = await TestHelpers.CreateAuthorizedClientAsync(_factory, requestedRole: "Teacher");

        var response = await client.PostAsJsonAsync("/api/admin/teachers", new
        {
            userId = Guid.NewGuid(),
            employeeId = "EMP-999",
            firstName = "Someone",
            lastName = "Else",
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
