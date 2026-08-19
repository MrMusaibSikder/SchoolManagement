namespace CourseHub.Application.Features.Batches.Dtos;

/// <summary>
/// A Batch always belongs to an existing Course — see
/// BatchService.CreateAsync for the validation (course must exist, and
/// must be active — creating a new batch under a soft-deleted course is
/// rejected).
/// </summary>
public record CreateBatchRequest(Guid CourseId, string Name, string Code, DateTime StartDate, int? Capacity);
