using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Users;
using CourseHub.Application.Features.Users.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Exceptions;
using Moq;

namespace CourseHub.UnitTests.Features.Users;

public class UserManagementServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IRoleRepository> _roleRepository = new();
    private readonly Mock<IUserRoleRepository> _userRoleRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UserManagementService _sut;

    public UserManagementServiceTests()
    {
        _sut = new UserManagementService(_userRepository.Object, _roleRepository.Object, _userRoleRepository.Object, _unitOfWork.Object);
    }

    private static User CreateUser() => User.Create("user@example.com", "hashed", "Rahim", "Uddin");

    [Fact]
    public async Task AssignRoleAsync_SuperAdmin_IsRejectedRegardlessOfCasing()
    {
        var user = CreateUser();
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Casing shouldn't matter — the guard must not be bypassable by
        // sending "superadmin" or "SUPERADMIN".
        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => _sut.AssignRoleAsync(user.Id, new AssignRoleRequest("superadmin")));

        Assert.Contains("SuperAdmin cannot be assigned", ex.Message);
        _roleRepository.Verify(x => x.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AssignRoleAsync_RoleDoesNotExist_ThrowsNotFoundException()
    {
        var user = CreateUser();
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _roleRepository.Setup(x => x.GetByNameAsync("Ghost", It.IsAny<CancellationToken>())).ReturnsAsync((Role?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.AssignRoleAsync(user.Id, new AssignRoleRequest("Ghost")));
    }

    [Fact]
    public async Task AssignRoleAsync_AlreadyAssigned_IsIdempotent_DoesNotInsertAgain()
    {
        var user = CreateUser();
        var role = Role.CreateSystemRole("Teacher");
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _roleRepository.Setup(x => x.GetByNameAsync("Teacher", It.IsAny<CancellationToken>())).ReturnsAsync(role);
        _userRoleRepository.Setup(x => x.ExistsAsync(user.Id, role.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _userRoleRepository
            .Setup(x => x.GetRoleNamesForUserAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Teacher" });

        var result = await _sut.AssignRoleAsync(user.Id, new AssignRoleRequest("Teacher"));

        Assert.Contains("Teacher", result.Roles);
        _userRoleRepository.Verify(x => x.AddAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AssignRoleAsync_NotYetAssigned_AddsRoleAndSaves()
    {
        var user = CreateUser();
        var role = Role.CreateSystemRole("Teacher");
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _roleRepository.Setup(x => x.GetByNameAsync("Teacher", It.IsAny<CancellationToken>())).ReturnsAsync(role);
        _userRoleRepository.Setup(x => x.ExistsAsync(user.Id, role.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _userRoleRepository
            .Setup(x => x.GetRoleNamesForUserAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Student", "Teacher" });

        var result = await _sut.AssignRoleAsync(user.Id, new AssignRoleRequest("Teacher"));

        Assert.Contains("Teacher", result.Roles);
        _userRoleRepository.Verify(x => x.AddAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveRoleAsync_NotCurrentlyAssigned_StillSucceeds()
    {
        var user = CreateUser();
        var role = Role.CreateSystemRole("Teacher");
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _roleRepository.Setup(x => x.GetByNameAsync("Teacher", It.IsAny<CancellationToken>())).ReturnsAsync(role);
        _userRoleRepository
            .Setup(x => x.GetRoleNamesForUserAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());

        // IUserRoleRepository.RemoveAsync is itself documented as a no-op
        // when the link doesn't exist — this just confirms the service
        // doesn't add its own error on top of that.
        var result = await _sut.RemoveRoleAsync(user.Id, "Teacher");

        Assert.DoesNotContain("Teacher", result.Roles);
        _userRoleRepository.Verify(x => x.RemoveAsync(user.Id, role.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SuspendAsync_SetsStatusToSuspended()
    {
        var user = CreateUser();
        Assert.Equal("Active", user.Status.ToString());
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userRoleRepository
            .Setup(x => x.GetRoleNamesForUserAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());

        var result = await _sut.SuspendAsync(user.Id);

        Assert.Equal("Suspended", result.Status);
    }
}
