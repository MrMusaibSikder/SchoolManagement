using System.Net;
using System.Net.Http.Json;

namespace CourseHub.IntegrationTests;

public class BatchesEndpointsTests : IClassFixture<CourseHubApiFactory>
{
    private readonly CourseHubApiFactory _factory;

    public BatchesEndpointsTests(CourseHubApiFactory factory)
    {
        _factory = factory;
    }

    private record CourseResponseDto(Guid Id, string Name, string Code, bool IsActive);

    private record BatchResponseDto(Guid Id, Guid CourseId, string Name, string Code, bool IsActive, int? Capacity);

    private static async Task<CourseResponseDto> CreateCourseAsync(HttpClient superAdmin)
    {
        var code = $"C-{Guid.NewGuid():N}"[..12];
        var response = await superAdmin.PostAsJsonAsync(
            "/api/admin/courses",
            new { name = "Batch Test Course", code, durationInMonths = 3, description = (string?)null });
        return (await response.Content.ReadFromJsonAsync<CourseResponseDto>())!;
    }

    [Fact]
    public async Task CreateBatch_UnderInactiveCourse_ReturnsBadRequest()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        var course = await CreateCourseAsync(superAdmin);
        await superAdmin.DeleteAsync($"/api/admin/courses/{course.Id}");

        var response = await superAdmin.PostAsJsonAsync("/api/admin/batches", new
        {
            courseId = course.Id,
            name = "Should Not Be Created",
            code = $"B-{Guid.NewGuid():N}"[..12],
            startDate = DateTime.UtcNow.AddDays(30),
            capacity = (int?)null,
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateBatch_UnderActiveCourse_Succeeds()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        var course = await CreateCourseAsync(superAdmin);

        var response = await superAdmin.PostAsJsonAsync("/api/admin/batches", new
        {
            courseId = course.Id,
            name = "Morning Batch",
            code = $"B-{Guid.NewGuid():N}"[..12],
            startDate = DateTime.UtcNow.AddDays(30),
            capacity = 25,
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<BatchResponseDto>();
        Assert.NotNull(created);
        Assert.Equal(course.Id, created!.CourseId);
        Assert.Equal(25, created.Capacity);
    }

    [Fact]
    public async Task ListBatches_FilteredByCourseId_ReturnsOnlyThatCoursesBatches()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        var courseA = await CreateCourseAsync(superAdmin);
        var courseB = await CreateCourseAsync(superAdmin);

        await superAdmin.PostAsJsonAsync("/api/admin/batches", new
        {
            courseId = courseA.Id,
            name = "Course A Batch",
            code = $"B-{Guid.NewGuid():N}"[..12],
            startDate = DateTime.UtcNow.AddDays(10),
            capacity = (int?)null,
        });
        await superAdmin.PostAsJsonAsync("/api/admin/batches", new
        {
            courseId = courseB.Id,
            name = "Course B Batch",
            code = $"B-{Guid.NewGuid():N}"[..12],
            startDate = DateTime.UtcNow.AddDays(10),
            capacity = (int?)null,
        });

        var response = await superAdmin.GetFromJsonAsync<PagedResultDto<BatchResponseDto>>($"/api/admin/batches?courseId={courseA.Id}");

        Assert.NotNull(response);
        Assert.All(response!.Items, b => Assert.Equal(courseA.Id, b.CourseId));
    }
}
