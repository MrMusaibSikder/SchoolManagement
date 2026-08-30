using System.Net;
using System.Net.Http.Json;

namespace CourseHub.IntegrationTests;

public class RolePermissionsEndpointsTests : IClassFixture<CourseHubApiFactory>
{
    private readonly CourseHubApiFactory _factory;

    public RolePermissionsEndpointsTests(CourseHubApiFactory factory)
    {
        _factory = factory;
    }

    private record RoleResponseDto(Guid Id, string Name, bool IsSystemRole);

    private record PermissionResponseDto(Guid Id, string Name, string Resource, string Action);

    private record RolePermissionsResponseDto(Guid RoleId, string RoleName, string[] Permissions);

    [Fact]
    public async Task GetRoles_AsSuperAdmin_ReturnsSeededSystemRoles()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);

        var roles = await superAdmin.GetFromJsonAsync<List<RoleResponseDto>>("/api/admin/roles");

        Assert.NotNull(roles);
        foreach (var expected in new[] { "SuperAdmin", "Admin", "Teacher", "Student" })
        {
            Assert.Contains(roles!, r => r.Name == expected && r.IsSystemRole);
        }
    }

    [Fact]
    public async Task GetPermissions_ReturnsSeededCatalog()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);

        var permissions = await superAdmin.GetFromJsonAsync<List<PermissionResponseDto>>("/api/admin/permissions");

        Assert.NotNull(permissions);
        Assert.Contains(permissions!, p => p.Name == "courses.view");
        Assert.Contains(permissions!, p => p.Name == "roles.manage");
    }

    [Fact]
    public async Task GetRolePermissions_ForSuperAdminRole_IncludesEveryPermission()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);

        var roles = await superAdmin.GetFromJsonAsync<List<RoleResponseDto>>("/api/admin/roles");
        var superAdminRole = roles!.Single(r => r.Name == "SuperAdmin");
        var permissions = await superAdmin.GetFromJsonAsync<List<PermissionResponseDto>>("/api/admin/permissions");

        var rolePermissions = await superAdmin.GetFromJsonAsync<RolePermissionsResponseDto>(
            $"/api/admin/roles/{superAdminRole.Id}/permissions");

        // SuperAdmin is seeded with every permission in the catalog as
        // real RolePermission rows (see DatabaseSeeder) — not just a
        // runtime bypass — so this must hold true even before the
        // handler-level IsInRole(SuperAdmin) safety net ever kicks in.
        Assert.NotNull(rolePermissions);
        foreach (var permission in permissions!)
        {
            Assert.Contains(permission.Name, rolePermissions!.Permissions);
        }
    }

    [Fact]
    public async Task AssignThenRemovePermission_OnANonSystemRole()
    {
        var superAdmin = await TestHelpers.CreateSuperAdminClientAsync(_factory);

        // Bootstrap a fresh, non-system role to assign/remove against —
        // avoids mutating one of the seeded system roles' permission set
        // as a side effect of running this test.
        var (_, targetAuth) = await TestHelpers.CreateAuthorizedClientAsync(_factory);
        var assignRoleResponse = await superAdmin.PostAsJsonAsync($"/api/admin/users/{targetAuth.User.Id}/roles", new { roleName = "Teacher" });
        Assert.Equal(HttpStatusCode.OK, assignRoleResponse.StatusCode);

        var roles = await superAdmin.GetFromJsonAsync<List<RoleResponseDto>>("/api/admin/roles");
        var teacherRole = roles!.Single(r => r.Name == "Teacher");

        var assignResponse = await superAdmin.PostAsJsonAsync(
            $"/api/admin/roles/{teacherRole.Id}/permissions",
            new { permissionName = "students.view" });
        Assert.Equal(HttpStatusCode.OK, assignResponse.StatusCode);
        var afterAssign = await assignResponse.Content.ReadFromJsonAsync<RolePermissionsResponseDto>();
        Assert.Contains("students.view", afterAssign!.Permissions);

        var removeResponse = await superAdmin.DeleteAsync($"/api/admin/roles/{teacherRole.Id}/permissions/students.view");
        Assert.Equal(HttpStatusCode.OK, removeResponse.StatusCode);
        var afterRemove = await removeResponse.Content.ReadFromJsonAsync<RolePermissionsResponseDto>();
        Assert.DoesNotContain("students.view", afterRemove!.Permissions);
    }

    [Fact]
    public async Task GetRoles_AsStudent_ReturnsForbidden()
    {
        var (client, _) = await TestHelpers.CreateAuthorizedClientAsync(_factory);

        var response = await client.GetAsync("/api/admin/roles");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
