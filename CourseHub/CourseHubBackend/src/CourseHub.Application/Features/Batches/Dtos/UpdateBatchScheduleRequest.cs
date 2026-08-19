namespace CourseHub.Application.Features.Batches.Dtos;

public record UpdateBatchScheduleRequest(DateTime StartDate, DateTime? EndDate);
