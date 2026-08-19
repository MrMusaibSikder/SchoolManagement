using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Features.Batches.Dtos;

namespace CourseHub.Application.Features.Batches;

public interface IBatchService
{
    Task<PagedResult<BatchResponse>> SearchAsync(string? search, Guid? courseId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<BatchResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<BatchResponse> CreateAsync(CreateBatchRequest request, CancellationToken cancellationToken = default);

    Task<BatchResponse> UpdateAsync(Guid id, UpdateBatchRequest request, CancellationToken cancellationToken = default);

    Task<BatchResponse> UpdateScheduleAsync(Guid id, UpdateBatchScheduleRequest request, CancellationToken cancellationToken = default);

    Task<BatchResponse> UpdateCapacityAsync(Guid id, UpdateBatchCapacityRequest request, CancellationToken cancellationToken = default);

    Task<BatchResponse> ActivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<BatchResponse> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-delete: deactivates the batch rather than removing the row.
    /// See BatchService.DeleteAsync for why.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
