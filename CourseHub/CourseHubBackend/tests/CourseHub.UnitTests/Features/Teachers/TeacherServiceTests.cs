using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Teachers;
using CourseHub.Application.Features.Teachers.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Exceptions;
using Moq;

namespace CourseHub.UnitTests.Features.Teachers;

public class TeacherServiceTests
{
    private readonly Mock<ITeacherRepository> _teacherRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IUserRoleRepository> _userRoleRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly TeacherService _sut;

    public TeacherServiceTests()
    {
        _sut = new TeacherService(_teacherRepository.Object, _userRepository.Object, _userRoleRepository.Object, _unitOfWork.Object);
    }

    private static User CreateUser() => User.Create("teacher@example.com", "hashed", "Rahim", "Uddin");

    [Fact]
    public async Task CreateAsync_UserDoesNotExist_ThrowsNotFoundException()
    {
        _userRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var request = new CreateTeacherRequest(Guid.NewGuid(), "EMP-001", "Rahim", "Uddin");

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_UserDoesNotHaveTeacherRole_ThrowsValidationException()
    {
        var user = CreateUser();
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userRoleRepository
            .Setup(x => x.GetRoleNamesForUserAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Student" });

        var request = new CreateTeacherRequest(user.Id, "EMP-001", "Rahim", "Uddin");

        var ex = await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(request));
        Assert.Contains("does not have the Teacher role", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_UserAlreadyHasProfile_ThrowsValidationException()
    {
        var user = CreateUser();
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userRoleRepository
            .Setup(x => x.GetRoleNamesForUserAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Teacher" });
        _teacherRepository.Setup(x => x.ExistsByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var request = new CreateTeacherRequest(user.Id, "EMP-001", "Rahim", "Uddin");

        var ex = await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(request));
        Assert.Contains("already has a teacher profile", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_EmployeeIdTaken_ThrowsValidationException()
    {
        var user = CreateUser();
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userRoleRepository
            .Setup(x => x.GetRoleNamesForUserAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Teacher" });
        _teacherRepository.Setup(x => x.ExistsByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _teacherRepository
            .Setup(x => x.ExistsByEmployeeIdAsync("EMP-001", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new CreateTeacherRequest(user.Id, "EMP-001", "Rahim", "Uddin");

        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_AllChecksPass_CreatesTeacherProfile()
    {
        var user = CreateUser();
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userRoleRepository
            .Setup(x => x.GetRoleNamesForUserAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Teacher" });
        _teacherRepository.Setup(x => x.ExistsByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _teacherRepository
            .Setup(x => x.ExistsByEmployeeIdAsync("EMP-001", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CreateTeacherRequest(user.Id, "EMP-001", "Rahim", "Uddin");

        var result = await _sut.CreateAsync(request);

        Assert.Equal(user.Id, result.UserId);
        Assert.Equal("EMP-001", result.EmployeeId);
        Assert.True(result.IsActive);
        _teacherRepository.Verify(x => x.AddAsync(It.IsAny<Teacher>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeactivatesTeacher()
    {
        var teacher = Teacher.Create(Guid.NewGuid(), "EMP-002", "Karim", "Islam");
        _teacherRepository.Setup(x => x.GetByIdAsync(teacher.Id, It.IsAny<CancellationToken>())).ReturnsAsync(teacher);

        await _sut.DeleteAsync(teacher.Id);

        Assert.False(teacher.IsActive);
    }
}
