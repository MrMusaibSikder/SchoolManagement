using System.Net;
using System.Net.Http.Json;

namespace CourseHub.IntegrationTests;

public class EnrollmentsEndpointsTests : IClassFixture<CourseHubApiFactory>
{
    private readonly CourseHubApiFactory _factory;

    public EnrollmentsEndpointsTests(CourseHubApiFactory factory)
    {
        _factory = factory;
    }

    private record CourseResponseDto(Guid Id);

    private record BatchResponseDto(Guid Id);

    private record StudentResponseDto(Guid Id);

    private record EnrollmentResponseDto(Guid Id, Guid StudentId, Guid BatchId, string Status);

    /// <summary>
    /// Full setup chain for enrollment tests: a course, a batch under it
    /// (with the given capacity), and a promoted Student profile —
    /// everything an enrollment needs to exist first.
    /// </summary>
    private static async Task<(Guid StudentId, Guid BatchId)> SetUpStudentAndBatchAsync(HttpClient superAdmin, CourseHubApiFactory factory, int? capacity = null)
    {
        var courseResponse = await superAdmin.PostAsJsonAsync(
            "/api/admin/courses",
            new { name = "Enrollment Test Course", code = $"C-{Guid.NewGuid():N}"[..12], durationInMonths = 3, description = (string?)null });
        var course = (await courseResponse.Content.ReadFromJsonAsync<CourseResponseDto>())!;

        var batchResponse = await superAdmin.PostAsJsonAsync("/api/admin/batches", new
        {
            courseId = course.Id,
            name = "Enrollment Test Batch",
            code = $"B-{Guid.NewGuid():N}"[..12],
            startDate = DateTime.UtcNow.AddDays(10),
            capacity,
        });
        var batch = (await batchResponse.Content.ReadFromJsonAsync<BatchResponseDto>())!;

        var (_, studentAuth) = await TestHelpers.CreateAuthorizedClientAsync(factory);
        var studentResponse = await superAdmin.PostAsJsonAsync("/api/admin/students", new
        {
            userId = studentAuth.User.Id,
            studentId = $"STU-{Guid.NewGuid():N}"[..12],
            firstName = "Karim",
            lastName = "Islam",
        });
        var student = (await studentResponse.Content.ReadFromJsonAsync<StudentResponseDto>())!;

        return (student.Id, batch.Id);
    }

    [Fact]
    public async Task CreateEnrollment_Duplicate_ReturnsBadRequest()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        var (studentId, batchId) = await SetUpStudentAndBatchAsync(superAdmin, _factory);

        var first = await superAdmin.PostAsJsonAsync("/api/admin/enrollments", new { studentId, batchId });
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await superAdmin.PostAsJsonAsync("/api/admin/enrollments", new { studentId, batchId });
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task CreateEnrollment_BeyondCapacity_ReturnsBadRequest()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        var (firstStudentId, batchId) = await SetUpStudentAndBatchAsync(superAdmin, _factory, capacity: 1);

        var first = await superAdmin.PostAsJsonAsync("/api/admin/enrollments", new { studentId = firstStudentId, batchId });
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        // A second, different student trying to enroll into the same
        // now-full (capacity: 1) batch.
        var (secondStudentId, _) = await SetUpStudentAndBatchAsync(superAdmin, _factory);

        var second = await superAdmin.PostAsJsonAsync("/api/admin/enrollments", new { studentId = secondStudentId, batchId });
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task Enrollment_FullLifecycle_PendingToActiveToCompleted()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        var (studentId, batchId) = await SetUpStudentAndBatchAsync(superAdmin, _factory);

        var createResponse = await superAdmin.PostAsJsonAsync("/api/admin/enrollments", new { studentId, batchId });
        var enrollment = (await createResponse.Content.ReadFromJsonAsync<EnrollmentResponseDto>())!;
        Assert.Equal("Pending", enrollment.Status);

        var approveResponse = await superAdmin.PostAsync($"/api/admin/enrollments/{enrollment.Id}/approve", content: null);
        Assert.Equal(HttpStatusCode.OK, approveResponse.StatusCode);
        var approved = await approveResponse.Content.ReadFromJsonAsync<EnrollmentResponseDto>();
        Assert.Equal("Active", approved!.Status);

        // Approving an already-Active enrollment is an invalid state
        // transition — the domain rejects it (mapped to 400 by
        // GlobalExceptionHandler), not silently ignored.
        var reapproveResponse = await superAdmin.PostAsync($"/api/admin/enrollments/{enrollment.Id}/approve", content: null);
        Assert.Equal(HttpStatusCode.BadRequest, reapproveResponse.StatusCode);

        var completeResponse = await superAdmin.PostAsync($"/api/admin/enrollments/{enrollment.Id}/complete", content: null);
        Assert.Equal(HttpStatusCode.OK, completeResponse.StatusCode);
        var completed = await completeResponse.Content.ReadFromJsonAsync<EnrollmentResponseDto>();
        Assert.Equal("Completed", completed!.Status);
    }

    [Fact]
    public async Task DeleteEnrollment_MapsToCancelRatherThanRemovingTheRow()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);
        var (studentId, batchId) = await SetUpStudentAndBatchAsync(superAdmin, _factory);

        var createResponse = await superAdmin.PostAsJsonAsync("/api/admin/enrollments", new { studentId, batchId });
        var enrollment = (await createResponse.Content.ReadFromJsonAsync<EnrollmentResponseDto>())!;

        var deleteResponse = await superAdmin.DeleteAsync($"/api/admin/enrollments/{enrollment.Id}");

        // Unlike every other admin controller's DELETE (204 No Content),
        // this one returns 200 with the updated body — because it's
        // literally the same Cancel() operation as POST .../cancel, not
        // a row removal (Enrollment has no separate soft-delete state).
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        var cancelled = await deleteResponse.Content.ReadFromJsonAsync<EnrollmentResponseDto>();
        Assert.Equal("Cancelled", cancelled!.Status);

        var getResponse = await superAdmin.GetAsync($"/api/admin/enrollments/{enrollment.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateEnrollment_AsTeacherWithoutCreatePermission_ReturnsForbidden()
    {
        // Teacher has enrollments.view/update by default, not
        // enrollments.create — only Admin/SuperAdmin can create one.
        var (client, _) = await TestHelpers.CreateAuthorizedClientAsync(_factory, requestedRole: "Teacher");

        var response = await client.PostAsJsonAsync("/api/admin/enrollments", new { studentId = Guid.NewGuid(), batchId = Guid.NewGuid() });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
