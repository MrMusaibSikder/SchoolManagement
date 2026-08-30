using System.Net;
using System.Net.Http.Json;

namespace CourseHub.IntegrationTests;

public class MeEndpointsTests : IClassFixture<CourseHubApiFactory>
{
    private readonly CourseHubApiFactory _factory;

    public MeEndpointsTests(CourseHubApiFactory factory)
    {
        _factory = factory;
    }

    private record CourseResponseDto(Guid Id);

    private record BatchResponseDto(Guid Id);

    private record StudentResponseDto(Guid Id);

    private record EnrollmentResponseDto(Guid Id, Guid StudentId, Guid BatchId, string Status);

    [Fact]
    public async Task GetMyEnrollments_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/me/enrollments");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMyEnrollments_CallerHasNoStudentProfile_ReturnsNotFound()
    {
        // A freshly-registered Teacher (or an unpromoted Student) has no
        // Student profile row at all yet.
        var (client, _) = await TestHelpers.CreateAuthorizedClientAsync(_factory, requestedRole: "Teacher");

        var response = await client.GetAsync("/api/me/enrollments");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMyEnrollments_AfterBeingEnrolled_ReturnsOwnEnrollmentOnly()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);

        // The student's own client — used both to register and, later,
        // to call the self-service endpoint with their own token.
        var studentClient = _factory.CreateClient();
        var studentAuth = await TestHelpers.RegisterAsync(studentClient);

        var courseResponse = await superAdmin.PostAsJsonAsync(
            "/api/admin/courses",
            new { name = "Me Endpoint Test Course", code = $"C-{Guid.NewGuid():N}"[..12], durationInMonths = 3, description = (string?)null });
        var course = (await courseResponse.Content.ReadFromJsonAsync<CourseResponseDto>())!;

        var batchResponse = await superAdmin.PostAsJsonAsync("/api/admin/batches", new
        {
            courseId = course.Id,
            name = "Me Endpoint Test Batch",
            code = $"B-{Guid.NewGuid():N}"[..12],
            startDate = DateTime.UtcNow.AddDays(5),
            capacity = (int?)null,
        });
        var batch = (await batchResponse.Content.ReadFromJsonAsync<BatchResponseDto>())!;

        var studentProfileResponse = await superAdmin.PostAsJsonAsync("/api/admin/students", new
        {
            userId = studentAuth.User.Id,
            studentId = $"STU-{Guid.NewGuid():N}"[..12],
            firstName = "Karim",
            lastName = "Islam",
        });
        var studentProfile = (await studentProfileResponse.Content.ReadFromJsonAsync<StudentResponseDto>())!;

        await superAdmin.PostAsJsonAsync("/api/admin/enrollments", new { studentId = studentProfile.Id, batchId = batch.Id });

        studentClient.AuthorizeWith(studentAuth.AccessToken);
        var response = await studentClient.GetAsync("/api/me/enrollments");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<EnrollmentResponseDto>>();
        Assert.NotNull(result);
        Assert.Equal(1, result!.TotalCount);
        Assert.Equal(studentProfile.Id, result.Items[0].StudentId);
        Assert.Equal(batch.Id, result.Items[0].BatchId);
        Assert.Equal("Pending", result.Items[0].Status);
    }
}
