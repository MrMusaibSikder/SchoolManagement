using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Enrollments;
using CourseHub.Application.Features.Enrollments.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;
using CourseHub.Domain.Exceptions;
using Moq;

namespace CourseHub.UnitTests.Features.Enrollments;

public class EnrollmentServiceTests
{
    private readonly Mock<IEnrollmentRepository> _enrollmentRepository = new();
    private readonly Mock<IStudentRepository> _studentRepository = new();
    private readonly Mock<IBatchRepository> _batchRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly EnrollmentService _sut;

    public EnrollmentServiceTests()
    {
        _sut = new EnrollmentService(_enrollmentRepository.Object, _studentRepository.Object, _batchRepository.Object, _unitOfWork.Object);
    }

    private static Student CreateActiveStudent() => Student.Create(Guid.NewGuid(), "STU-001", "Karim", "Islam");

    private static Batch CreateActiveBatch(int? capacity = null) =>
        Batch.Create(Guid.NewGuid(), "Morning Batch", "B-01", DateTime.UtcNow, capacity);

    [Fact]
    public async Task CreateAsync_StudentIsInactive_ThrowsValidationException()
    {
        var student = CreateActiveStudent();
        student.Deactivate();
        _studentRepository.Setup(x => x.GetByIdAsync(student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(student);

        var request = new CreateEnrollmentRequest(student.Id, Guid.NewGuid());

        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_BatchIsInactive_ThrowsValidationException()
    {
        var student = CreateActiveStudent();
        var batch = CreateActiveBatch();
        batch.Deactivate();
        _studentRepository.Setup(x => x.GetByIdAsync(student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(student);
        _batchRepository.Setup(x => x.GetByIdAsync(batch.Id, It.IsAny<CancellationToken>())).ReturnsAsync(batch);

        var request = new CreateEnrollmentRequest(student.Id, batch.Id);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_AlreadyEnrolledInBatch_ThrowsValidationException()
    {
        var student = CreateActiveStudent();
        var batch = CreateActiveBatch();
        _studentRepository.Setup(x => x.GetByIdAsync(student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(student);
        _batchRepository.Setup(x => x.GetByIdAsync(batch.Id, It.IsAny<CancellationToken>())).ReturnsAsync(batch);
        _enrollmentRepository
            .Setup(x => x.ExistsForStudentAndBatchAsync(student.Id, batch.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new CreateEnrollmentRequest(student.Id, batch.Id);

        var ex = await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(request));
        Assert.Contains("already enrolled", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_BatchAtFullCapacity_ThrowsValidationException()
    {
        var student = CreateActiveStudent();
        var batch = CreateActiveBatch(capacity: 2);
        _studentRepository.Setup(x => x.GetByIdAsync(student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(student);
        _batchRepository.Setup(x => x.GetByIdAsync(batch.Id, It.IsAny<CancellationToken>())).ReturnsAsync(batch);
        _enrollmentRepository
            .Setup(x => x.ExistsForStudentAndBatchAsync(student.Id, batch.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        // Two seats already occupied (Pending/Active) out of a capacity of 2.
        _enrollmentRepository
            .Setup(x => x.CountForBatchByStatusesAsync(
                batch.Id,
                It.Is<IReadOnlyList<EnrollmentStatus>>(s => s.Contains(EnrollmentStatus.Pending) && s.Contains(EnrollmentStatus.Active)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var request = new CreateEnrollmentRequest(student.Id, batch.Id);

        var ex = await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(request));
        Assert.Contains("full capacity", ex.Message);

        _enrollmentRepository.Verify(x => x.AddAsync(It.IsAny<Enrollment>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_BatchHasFreeSeat_CreatesEnrollmentAsPending()
    {
        var student = CreateActiveStudent();
        var batch = CreateActiveBatch(capacity: 2);
        _studentRepository.Setup(x => x.GetByIdAsync(student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(student);
        _batchRepository.Setup(x => x.GetByIdAsync(batch.Id, It.IsAny<CancellationToken>())).ReturnsAsync(batch);
        _enrollmentRepository
            .Setup(x => x.ExistsForStudentAndBatchAsync(student.Id, batch.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _enrollmentRepository
            .Setup(x => x.CountForBatchByStatusesAsync(batch.Id, It.IsAny<IReadOnlyList<EnrollmentStatus>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var request = new CreateEnrollmentRequest(student.Id, batch.Id);

        var result = await _sut.CreateAsync(request);

        Assert.Equal("Pending", result.Status);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_UnlimitedCapacityBatch_SkipsCapacityCheckEntirely()
    {
        var student = CreateActiveStudent();
        var batch = CreateActiveBatch(capacity: null);
        _studentRepository.Setup(x => x.GetByIdAsync(student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(student);
        _batchRepository.Setup(x => x.GetByIdAsync(batch.Id, It.IsAny<CancellationToken>())).ReturnsAsync(batch);
        _enrollmentRepository
            .Setup(x => x.ExistsForStudentAndBatchAsync(student.Id, batch.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CreateEnrollmentRequest(student.Id, batch.Id);

        await _sut.CreateAsync(request);

        _enrollmentRepository.Verify(
            x => x.CountForBatchByStatusesAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyList<EnrollmentStatus>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ApproveAsync_PendingEnrollment_BecomesActive()
    {
        var enrollment = Enrollment.Create(Guid.NewGuid(), Guid.NewGuid());
        _enrollmentRepository.Setup(x => x.GetByIdAsync(enrollment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);

        var result = await _sut.ApproveAsync(enrollment.Id);

        Assert.Equal("Active", result.Status);
    }

    [Fact]
    public async Task ApproveAsync_AlreadyActiveEnrollment_ThrowsDomainException()
    {
        var enrollment = Enrollment.Create(Guid.NewGuid(), Guid.NewGuid());
        enrollment.Approve();
        _enrollmentRepository.Setup(x => x.GetByIdAsync(enrollment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);

        // The Pending -> Active -> Completed state machine is enforced in
        // the domain, not re-validated in the service — approving twice
        // must fail the same way calling Enrollment.Approve() twice would.
        await Assert.ThrowsAsync<DomainException>(() => _sut.ApproveAsync(enrollment.Id));
    }

    [Fact]
    public async Task CancelAsync_PendingEnrollment_BecomesCancelled()
    {
        var enrollment = Enrollment.Create(Guid.NewGuid(), Guid.NewGuid());
        _enrollmentRepository.Setup(x => x.GetByIdAsync(enrollment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);

        var result = await _sut.CancelAsync(enrollment.Id);

        Assert.Equal("Cancelled", result.Status);
    }
}
