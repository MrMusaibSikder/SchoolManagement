using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Me;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;
using CourseHub.Domain.Exceptions;
using Moq;

namespace CourseHub.UnitTests.Features.Me;

public class MeServiceTests
{
    private readonly Mock<IStudentRepository> _studentRepository = new();
    private readonly Mock<IEnrollmentRepository> _enrollmentRepository = new();
    private readonly Mock<IAssignmentRepository> _assignmentRepository = new();
    private readonly Mock<ISubmissionRepository> _submissionRepository = new();
    private readonly MeService _sut;

    public MeServiceTests()
    {
        _sut = new MeService(
            _studentRepository.Object,
            _enrollmentRepository.Object,
            _assignmentRepository.Object,
            _submissionRepository.Object);
    }

    [Fact]
    public async Task GetMyEnrollmentsAsync_CallerHasNoStudentProfile_ThrowsNotFoundException()
    {
        var userId = Guid.NewGuid();
        _studentRepository
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        // A Teacher/Admin account, or a Student-role user nobody has
        // promoted into a profile yet, both hit this path.
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetMyEnrollmentsAsync(userId, 1, 20));

        _enrollmentRepository.Verify(
            x => x.SearchAsync(It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<EnrollmentStatus?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetMyEnrollmentsAsync_ScopesSearchToCallersOwnStudentId()
    {
        var userId = Guid.NewGuid();
        var student = Student.Create(userId, "STU-001", "Karim", "Islam");
        var enrollment = Enrollment.Create(student.Id, Guid.NewGuid());

        _studentRepository.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(student);
        _enrollmentRepository
            .Setup(x => x.SearchAsync(student.Id, null, null, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Enrollment> { enrollment }, 1));

        var result = await _sut.GetMyEnrollmentsAsync(userId, 1, 20);

        Assert.Equal(1, result.TotalCount);
        Assert.Equal(student.Id, result.Items[0].StudentId);

        // Confirms the lookup is keyed off the caller's own Student.Id,
        // never an id supplied by the caller — there's no way to pass
        // someone else's studentId into this endpoint at all.
        _enrollmentRepository.Verify(
            x => x.SearchAsync(student.Id, null, null, 1, 20, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetMyEnrollmentsAsync_ClampsInvalidPagingInput()
    {
        var userId = Guid.NewGuid();
        var student = Student.Create(userId, "STU-002", "Nasrin", "Akter");
        _studentRepository.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(student);
        _enrollmentRepository
            .Setup(x => x.SearchAsync(student.Id, null, null, 1, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Enrollment>(), 0));

        var result = await _sut.GetMyEnrollmentsAsync(userId, page: -5, pageSize: 999);

        Assert.Equal(1, result.Page);
        Assert.Equal(100, result.PageSize);
    }
}
