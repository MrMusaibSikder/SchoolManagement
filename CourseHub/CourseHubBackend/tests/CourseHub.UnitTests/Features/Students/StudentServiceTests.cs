using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Students;
using CourseHub.Application.Features.Students.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Exceptions;
using Moq;

namespace CourseHub.UnitTests.Features.Students;

public class StudentServiceTests
{
    private readonly Mock<IStudentRepository> _studentRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IUserRoleRepository> _userRoleRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly StudentService _sut;

    public StudentServiceTests()
    {
        _sut = new StudentService(_studentRepository.Object, _userRepository.Object, _userRoleRepository.Object, _unitOfWork.Object);
    }

    private static User CreateUser() => User.Create("student@example.com", "hashed", "Karim", "Islam");

    [Fact]
    public async Task CreateAsync_UserDoesNotHaveStudentRole_ThrowsValidationException()
    {
        var user = CreateUser();
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userRoleRepository
            .Setup(x => x.GetRoleNamesForUserAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Teacher" });

        var request = new CreateStudentRequest(user.Id, "STU-001", "Karim", "Islam");

        var ex = await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(request));
        Assert.Contains("does not have the Student role", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_StudentIdTaken_ThrowsValidationException()
    {
        var user = CreateUser();
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userRoleRepository
            .Setup(x => x.GetRoleNamesForUserAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Student" });
        _studentRepository.Setup(x => x.ExistsByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _studentRepository
            .Setup(x => x.ExistsByStudentIdAsync("STU-001", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new CreateStudentRequest(user.Id, "STU-001", "Karim", "Islam");

        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_AllChecksPass_CreatesStudentProfile()
    {
        var user = CreateUser();
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userRoleRepository
            .Setup(x => x.GetRoleNamesForUserAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Student" });
        _studentRepository.Setup(x => x.ExistsByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _studentRepository
            .Setup(x => x.ExistsByStudentIdAsync("STU-001", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CreateStudentRequest(user.Id, "STU-001", "Karim", "Islam");

        var result = await _sut.CreateAsync(request);

        Assert.Equal(user.Id, result.UserId);
        Assert.Equal("STU-001", result.StudentId);
        _studentRepository.Verify(x => x.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateGuardianAsync_UpdatesGuardianFields()
    {
        var student = Student.Create(Guid.NewGuid(), "STU-002", "Nasrin", "Akter");
        _studentRepository.Setup(x => x.GetByIdAsync(student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(student);

        var result = await _sut.UpdateGuardianAsync(student.Id, new UpdateStudentGuardianRequest("Abdul Karim", "+8801XXXXXXXXX"));

        Assert.Equal("Abdul Karim", result.GuardianName);
        Assert.Equal("+8801XXXXXXXXX", result.GuardianPhone);
    }

    [Fact]
    public async Task DeleteAsync_DeactivatesStudent()
    {
        var student = Student.Create(Guid.NewGuid(), "STU-003", "Farida", "Begum");
        _studentRepository.Setup(x => x.GetByIdAsync(student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(student);

        await _sut.DeleteAsync(student.Id);

        Assert.False(student.IsActive);
    }
}
