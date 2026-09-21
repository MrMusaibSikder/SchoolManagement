using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Courses;
using CourseHub.Application.Features.Courses.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Exceptions;
using Moq;

namespace CourseHub.UnitTests.Features.Courses;

public class CourseServiceTests
{
    private readonly Mock<ICourseRepository> _courseRepository = new();
    private readonly Mock<ITeacherRepository> _teacherRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CourseService _sut;

    public CourseServiceTests()
    {
        _sut = new CourseService(_courseRepository.Object, _teacherRepository.Object, _unitOfWork.Object);
    }

    private static Course CreateCourse(string code = "FSWD-01") => Course.Create("Full-Stack Web Dev", code, 6);

    [Fact]
    public async Task CreateAsync_CodeAlreadyTaken_ThrowsValidationException()
    {
        _courseRepository
            .Setup(x => x.ExistsByCodeAsync("FSWD-01", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new CreateCourseRequest("Full-Stack Web Dev", "FSWD-01", 6, null);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(request));

        _courseRepository.Verify(x => x.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_CodeAvailable_AddsCourseAndSaves()
    {
        _courseRepository
            .Setup(x => x.ExistsByCodeAsync("FSWD-01", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CreateCourseRequest("Full-Stack Web Dev", "FSWD-01", 6, "A great course");

        var result = await _sut.CreateAsync(request);

        Assert.Equal("FSWD-01", result.Code);
        Assert.True(result.IsActive);
        Assert.False(result.IsPublic);
        _courseRepository.Verify(x => x.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_CourseNotFound_ThrowsNotFoundException()
    {
        _courseRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course?)null);

        var request = new UpdateCourseRequest("New Name", "NEW-01", 3, null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(Guid.NewGuid(), request));
    }

    [Fact]
    public async Task UpdateAsync_CodeTakenByAnotherCourse_ThrowsValidationException()
    {
        var course = CreateCourse();
        _courseRepository.Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>())).ReturnsAsync(course);
        _courseRepository
            .Setup(x => x.ExistsByCodeAsync("TAKEN", course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new UpdateCourseRequest("Renamed", "TAKEN", 6, null);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.UpdateAsync(course.Id, request));
    }

    [Fact]
    public async Task DeleteAsync_DeactivatesRatherThanRemoving()
    {
        var course = CreateCourse();
        Assert.True(course.IsActive);
        _courseRepository.Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>())).ReturnsAsync(course);

        await _sut.DeleteAsync(course.Id);

        // The soft-delete contract: the entity is deactivated, never
        // removed from the repository (no Remove/Delete call exists on
        // ICourseRepository at all — there's nothing to verify wasn't
        // called because the interface doesn't expose it).
        Assert.False(course.IsActive);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ClampsInvalidPagingInput()
    {
        _courseRepository
            .Setup(x => x.SearchAsync(null, 1, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Course>(), 0));

        // page=0 and pageSize=100000 are both out-of-range inputs a
        // caller could pass via query string — the service must clamp
        // them rather than pass them straight to the database (page 0
        // would produce a negative Skip(), oversized pageSize would let
        // a caller force a full-table scan).
        var result = await _sut.SearchAsync(null, page: 0, pageSize: 100_000);

        Assert.Equal(1, result.Page);
        Assert.Equal(100, result.PageSize);
        _courseRepository.Verify(x => x.SearchAsync(null, 1, 100, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_MakesCoursePublic()
    {
        var course = CreateCourse();
        Assert.False(course.IsPublic);
        _courseRepository.Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>())).ReturnsAsync(course);

        var result = await _sut.PublishAsync(course.Id);

        Assert.True(result.IsPublic);
    }
}
