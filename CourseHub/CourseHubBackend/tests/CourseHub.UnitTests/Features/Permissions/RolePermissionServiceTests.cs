using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Permissions;
using CourseHub.Application.Features.Permissions.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Exceptions;
using Moq;

namespace CourseHub.UnitTests.Features.Permissions;

public class RolePermissionServiceTests
{
    private readonly Mock<IRoleRepository> _roleRepository = new();
    private readonly Mock<IPermissionRepository> _permissionRepository = new();
    private readonly Mock<IRolePermissionRepository> _rolePermissionRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly RolePermissionService _sut;

    public RolePermissionServiceTests()
    {
        _sut = new RolePermissionService(
            _roleRepository.Object,
            _permissionRepository.Object,
            _rolePermissionRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task GetPermissionsForRoleAsync_RoleDoesNotExist_ThrowsNotFoundException()
    {
        _roleRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Role?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetPermissionsForRoleAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task AssignPermissionAsync_PermissionDoesNotExist_ThrowsNotFoundException()
    {
        var role = Role.CreateSystemRole("Admin");
        _roleRepository.Setup(x => x.GetByIdAsync(role.Id, It.IsAny<CancellationToken>())).ReturnsAsync(role);
        _permissionRepository
            .Setup(x => x.GetByNameAsync("ghost.permission", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Permission?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.AssignPermissionAsync(role.Id, new AssignPermissionRequest("ghost.permission")));
    }

    [Fact]
    public async Task AssignPermissionAsync_AlreadyAssigned_IsIdempotent()
    {
        var role = Role.CreateSystemRole("Admin");
        var permission = Permission.Create("courses.view", "Course", "View");
        _roleRepository.Setup(x => x.GetByIdAsync(role.Id, It.IsAny<CancellationToken>())).ReturnsAsync(role);
        _permissionRepository.Setup(x => x.GetByNameAsync("courses.view", It.IsAny<CancellationToken>())).ReturnsAsync(permission);
        _rolePermissionRepository
            .Setup(x => x.ExistsAsync(role.Id, permission.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _rolePermissionRepository
            .Setup(x => x.GetPermissionNamesForRoleAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "courses.view" });

        var result = await _sut.AssignPermissionAsync(role.Id, new AssignPermissionRequest("courses.view"));

        Assert.Contains("courses.view", result.Permissions);
        _rolePermissionRepository.Verify(x => x.AddAsync(It.IsAny<RolePermission>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AssignPermissionAsync_NotYetAssigned_AddsLinkAndSaves()
    {
        var role = Role.CreateSystemRole("Admin");
        var permission = Permission.Create("courses.view", "Course", "View");
        _roleRepository.Setup(x => x.GetByIdAsync(role.Id, It.IsAny<CancellationToken>())).ReturnsAsync(role);
        _permissionRepository.Setup(x => x.GetByNameAsync("courses.view", It.IsAny<CancellationToken>())).ReturnsAsync(permission);
        _rolePermissionRepository
            .Setup(x => x.ExistsAsync(role.Id, permission.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _rolePermissionRepository
            .Setup(x => x.GetPermissionNamesForRoleAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "courses.view" });

        var result = await _sut.AssignPermissionAsync(role.Id, new AssignPermissionRequest("courses.view"));

        Assert.Contains("courses.view", result.Permissions);
        _rolePermissionRepository.Verify(x => x.AddAsync(It.IsAny<RolePermission>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemovePermissionAsync_PermissionDoesNotExist_ThrowsNotFoundException()
    {
        var role = Role.CreateSystemRole("Admin");
        _roleRepository.Setup(x => x.GetByIdAsync(role.Id, It.IsAny<CancellationToken>())).ReturnsAsync(role);
        _permissionRepository
            .Setup(x => x.GetByNameAsync("ghost.permission", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Permission?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.RemovePermissionAsync(role.Id, "ghost.permission"));
    }

    [Fact]
    public async Task GetRolesAsync_MapsEveryRoleToResponse()
    {
        var roles = new List<Role> { Role.CreateSystemRole("SuperAdmin"), Role.Create("CourseCoordinator") };
        _roleRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(roles);

        var result = await _sut.GetRolesAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Name == "SuperAdmin" && r.IsSystemRole);
        Assert.Contains(result, r => r.Name == "CourseCoordinator" && !r.IsSystemRole);
    }
}
