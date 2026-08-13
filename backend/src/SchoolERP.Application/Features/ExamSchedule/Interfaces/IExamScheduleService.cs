using SchoolERP.Application.Features.ExamSchedule.DTOs;

namespace SchoolERP.Application.Features.ExamSchedule.Interfaces;

/// <summary>
/// Business/service contract for ExamSchedule records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface IExamScheduleService
{
    /// <summary>Retrieves every ExamSchedule record (enriched with Exam/Class/Subject names).</summary>
    Task<IReadOnlyList<ExamScheduleDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single ExamSchedule record by id, or null if it does not exist.</summary>
    Task<ExamScheduleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves every schedule for a given exam, ordered by exam date.</summary>
    Task<IReadOnlyList<ExamScheduleDto>> GetSchedulesByExamAsync(int examId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves every schedule for a given class, optionally restricted to a single exam.</summary>
    Task<IReadOnlyList<ExamScheduleDto>> GetSchedulesByClassAsync(int classId, int? examId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves every schedule for subjects taught by a given teacher, optionally restricted to a single exam.</summary>
    Task<IReadOnlyList<ExamScheduleDto>> GetSchedulesByTeacherAsync(int teacherId, int? examId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new ExamSchedule record. The parent exam must exist and must
    /// not be Completed or Cancelled. The subject must not already be
    /// scheduled for the same exam + class, and the exam + class must not
    /// already have a different subject scheduled on the same date.
    /// </summary>
    Task<ExamScheduleDto> CreateAsync(CreateExamScheduleDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing ExamSchedule record. Not permitted once the parent exam is Completed or Cancelled.</summary>
    Task<ExamScheduleDto> UpdateAsync(int id, UpdateExamScheduleDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing ExamSchedule record. Not permitted once the parent exam is Completed or Cancelled.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
