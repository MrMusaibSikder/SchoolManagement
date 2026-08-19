namespace CourseHub.Application.Features.Batches.Dtos;

public record BatchResponse(
    Guid Id,
    Guid CourseId,
    string Name,
    string Code,
    DateTime StartDate,
    DateTime? EndDate,
    int? Capacity,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
