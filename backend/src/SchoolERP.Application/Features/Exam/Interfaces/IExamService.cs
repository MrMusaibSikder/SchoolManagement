using SchoolERP.Application.Features.Exam.DTOs;

namespace SchoolERP.Application.Features.Exam.Interfaces;

/// <summary>
/// Business/service contract for Exam records. Services return DTOs only and
/// encapsulate all business rules for this feature: duplicate/date validation,
/// the Draft/Published/Completed/Cancelled lifecycle, and dashboard/routine/
/// calendar composition. Deliberately excludes any marks/grade/result
/// concerns — those belong to the future Result module, which builds on top
/// of the relationships this module preserves (<c>ExamSchedule.Results</c>).
/// </summary>
public interface IExamService
{
    /// <summary>Retrieves every Exam record (enriched with ExamType/AcademicYear names).</summary>
    Task<IReadOnlyList<ExamDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single Exam record by id, or null if it does not exist.</summary>
    Task<ExamDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves full exam details (exam fields + every schedule under it).</summary>
    Task<ExamDetailsDto> GetExamDetailsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a lightweight summary of a single exam, for lists/dashboards.</summary>
    Task<ExamSummaryDto> GetExamSummaryAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves scheduling statistics for a single exam (subject/class counts, date span). No marks/results.</summary>
    Task<ExamStatisticsDto> GetExamStatisticsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new exam as a Draft. AcademicYear + ExamType + Name must be unique.</summary>
    Task<ExamDto> CreateAsync(CreateExamDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing exam. Only permitted while the exam is in Draft status.</summary>
    Task<ExamDto> UpdateAsync(int id, UpdateExamDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an exam. Not permitted once the exam is Completed or Cancelled.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Publishes a Draft exam, allowing its schedules to be managed and making it visible as upcoming.</summary>
    Task<ExamDto> PublishExamAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Marks a Published exam as Completed. Locks the exam and its schedules from further changes.</summary>
    Task<ExamDto> CompleteExamAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Cancels a Draft or Published exam. A cancelled exam cannot be published again (use <see cref="ReopenExamAsync"/> first).</summary>
    Task<ExamDto> CancelExamAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Reopens a Cancelled exam back to Draft, so it can be edited and re-published from scratch.</summary>
    Task<ExamDto> ReopenExamAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves aggregate exam-board statistics and highlights for the admin dashboard.</summary>
    Task<ExamDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves the next upcoming (published, not-yet-occurred) exams.</summary>
    Task<IReadOnlyList<UpcomingExamDto>> GetUpcomingExamsAsync(int count, CancellationToken cancellationToken = default);

    /// <summary>Retrieves every exam schedule falling within a date range, for a calendar/timetable view.</summary>
    Task<IReadOnlyList<ExamCalendarDto>> GetExamCalendarAsync(DateTime fromDate, DateTime toDate, int? classId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves the full subject-wise routine for a single exam.</summary>
    Task<ExamRoutineDto> GetExamRoutineAsync(int examId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves the subject-wise routine for a single class within a single exam.</summary>
    Task<ClassRoutineDto> GetClassRoutineAsync(int examId, int classId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves the personal exam routine for a single student (their class's schedules within the exam).</summary>
    Task<StudentRoutineDto> GetStudentRoutineAsync(int studentId, int examId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves the routine for a single teacher across the subjects they teach, optionally scoped to one exam.</summary>
    Task<TeacherRoutineDto> GetTeacherRoutineAsync(int teacherId, int? examId, CancellationToken cancellationToken = default);
}
