using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Batches.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Application.Features.Batches;

public class BatchService : IBatchService
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IBatchRepository _batchRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BatchService(IBatchRepository batchRepository, ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _batchRepository = batchRepository;
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<BatchResponse>> SearchAsync(string? search, Guid? courseId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var normalizedPage = Math.Max(page, 1);
        var normalizedPageSize = Math.Clamp(pageSize <= 0 ? DefaultPageSize : pageSize, 1, MaxPageSize);

        var (items, totalCount) = await _batchRepository.SearchAsync(search, courseId, normalizedPage, normalizedPageSize, cancellationToken);

        var responses = items.Select(ToResponse).ToList();

        return new PagedResult<BatchResponse>(responses, totalCount, normalizedPage, normalizedPageSize);
    }

    public async Task<BatchResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var batch = await RequireBatchAsync(id, cancellationToken);
        return ToResponse(batch);
    }

    public async Task<BatchResponse> CreateAsync(CreateBatchRequest request, CancellationToken cancellationToken = default)
    {
        // A Batch always belongs to an existing, active Course. Blocking
        // creation under an inactive (soft-deleted) course prevents
        // accidentally scheduling a new cohort under something the admin
        // already retired — a friendly 400/404 instead of a confusing
        // state to discover later.
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new NotFoundException("Course", request.CourseId);

        if (!course.IsActive)
        {
            throw new ValidationException($"Course '{course.Name}' is not active — reactivate it before adding new batches.");
        }

        await EnsureCodeIsAvailableAsync(request.Code, excludingId: null, cancellationToken);

        var batch = Batch.Create(request.CourseId, request.Name, request.Code, request.StartDate, request.Capacity);

        await _batchRepository.AddAsync(batch, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(batch);
    }

    public async Task<BatchResponse> UpdateAsync(Guid id, UpdateBatchRequest request, CancellationToken cancellationToken = default)
    {
        var batch = await RequireBatchAsync(id, cancellationToken);

        await EnsureCodeIsAvailableAsync(request.Code, excludingId: batch.Id, cancellationToken);

        batch.Update(request.Name, request.Code);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(batch);
    }

    public async Task<BatchResponse> UpdateScheduleAsync(Guid id, UpdateBatchScheduleRequest request, CancellationToken cancellationToken = default)
    {
        var batch = await RequireBatchAsync(id, cancellationToken);

        // Batch.SetSchedule throws a domain ValidationException if
        // EndDate < StartDate — that 400 comes straight from the domain,
        // not duplicated in the FluentValidation layer (see
        // UpdateBatchScheduleRequestValidator).
        batch.SetSchedule(request.StartDate, request.EndDate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(batch);
    }

    public async Task<BatchResponse> UpdateCapacityAsync(Guid id, UpdateBatchCapacityRequest request, CancellationToken cancellationToken = default)
    {
        var batch = await RequireBatchAsync(id, cancellationToken);

        batch.SetCapacity(request.Capacity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(batch);
    }

    public async Task<BatchResponse> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var batch = await RequireBatchAsync(id, cancellationToken);
        batch.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(batch);
    }

    public async Task<BatchResponse> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var batch = await RequireBatchAsync(id, cancellationToken);
        batch.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(batch);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var batch = await RequireBatchAsync(id, cancellationToken);

        // Soft delete — backed by a real FK, same as Student: Enrollment.
        // BatchId has DeleteBehavior.Restrict against Batch (see
        // EnrollmentConfiguration), specifically so enrollment history
        // survives. A hard delete on a batch with any enrollments would
        // fail with a raw FK-constraint DbUpdateException.
        batch.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureCodeIsAvailableAsync(string code, Guid? excludingId, CancellationToken cancellationToken)
    {
        var codeTaken = await _batchRepository.ExistsByCodeAsync(code, excludingId, cancellationToken);

        if (codeTaken)
        {
            throw new ValidationException($"A batch with code '{code}' already exists.");
        }
    }

    private async Task<Batch> RequireBatchAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _batchRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Batch", id);
    }

    private static BatchResponse ToResponse(Batch batch) => new(
        batch.Id,
        batch.CourseId,
        batch.Name,
        batch.Code,
        batch.StartDate,
        batch.EndDate,
        batch.Capacity,
        batch.IsActive,
        batch.CreatedAt,
        batch.UpdatedAt);
}
