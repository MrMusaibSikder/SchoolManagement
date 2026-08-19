namespace CourseHub.Application.Features.Batches.Dtos;

/// <summary>
/// Null Capacity means unlimited enrollment (see Batch.Capacity).
/// </summary>
public record UpdateBatchCapacityRequest(int? Capacity);
