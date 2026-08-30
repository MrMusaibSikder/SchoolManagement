using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Batches;
using CourseHub.Application.Features.Batches.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Exceptions;
using Moq;

namespace CourseHub.UnitTests.Features.Batches;

public class BatchServiceTests
{
    private readonly Mock<IBatchRepository> _batchRepository = new();
    private readonly Mock<ICourseRepository> _courseRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly BatchService _sut;

    public BatchServiceTests()
    {
        _sut = new BatchService(_batchRepository.Object, _courseRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task CreateAsync_CourseDoesNotExist_ThrowsNotFoundException()
    {
        _courseRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course?)null);

        var request = new CreateBatchRequest(Guid.NewGuid(), "Morning Batch", "B-01", DateTime.UtcNow, null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_CourseIsInactive_ThrowsValidationException()
    {
        var course = Course.Create("Web Dev", "WD-01", 6);
        course.Deactivate();
        _courseRepository.Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>())).ReturnsAsync(course);

        var request = new CreateBatchRequest(course.Id, "Morning Batch", "B-01", DateTime.UtcNow, null);

        var ex = await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(request));
        Assert.Contains("not active", ex.Message);

        _batchRepository.Verify(x => x.AddAsync(It.IsAny<Batch>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_CodeAlreadyTaken_ThrowsValidationException()
    {
        var course = Course.Create("Web Dev", "WD-01", 6);
        _courseRepository.Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>())).ReturnsAsync(course);
        _batchRepository
            .Setup(x => x.ExistsByCodeAsync("B-01", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new CreateBatchRequest(course.Id, "Morning Batch", "B-01", DateTime.UtcNow, null);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_ActiveCourseAndFreeCode_CreatesBatch()
    {
        var course = Course.Create("Web Dev", "WD-01", 6);
        _courseRepository.Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>())).ReturnsAsync(course);
        _batchRepository
            .Setup(x => x.ExistsByCodeAsync("B-01", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CreateBatchRequest(course.Id, "Morning Batch", "B-01", DateTime.UtcNow, 30);

        var result = await _sut.CreateAsync(request);

        Assert.Equal(course.Id, result.CourseId);
        Assert.Equal(30, result.Capacity);
        Assert.True(result.IsActive);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateScheduleAsync_EndDateBeforeStartDate_PropagatesDomainException()
    {
        var batch = Batch.Create(Guid.NewGuid(), "Morning Batch", "B-02", new DateTime(2027, 1, 1));
        _batchRepository.Setup(x => x.GetByIdAsync(batch.Id, It.IsAny<CancellationToken>())).ReturnsAsync(batch);

        // The StartDate <= EndDate rule lives in the domain
        // (Batch.SetSchedule) — the service doesn't duplicate it, so this
        // confirms the domain exception surfaces through unchanged.
        var request = new UpdateBatchScheduleRequest(new DateTime(2027, 6, 1), new DateTime(2027, 1, 1));

        await Assert.ThrowsAsync<ValidationException>(() => _sut.UpdateScheduleAsync(batch.Id, request));
    }

    [Fact]
    public async Task DeleteAsync_DeactivatesBatch()
    {
        var batch = Batch.Create(Guid.NewGuid(), "Morning Batch", "B-03", DateTime.UtcNow);
        _batchRepository.Setup(x => x.GetByIdAsync(batch.Id, It.IsAny<CancellationToken>())).ReturnsAsync(batch);

        await _sut.DeleteAsync(batch.Id);

        Assert.False(batch.IsActive);
    }
}
