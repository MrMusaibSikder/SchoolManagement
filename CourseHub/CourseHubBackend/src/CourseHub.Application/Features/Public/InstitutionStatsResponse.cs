namespace CourseHub.Application.Features.Public;

/// <summary>
/// Aggregate, non-identifying counts for the public landing page's
/// "stats" section (e.g. a frontend chart/graph, or plain numbers like
/// "500+ students, 40+ teachers"). Never exposes which teachers/students
/// exist — only totals.
/// </summary>
public record InstitutionStatsResponse(
    int TotalTeachers,
    int TotalStudents,
    int TotalCourses,
    int TotalActiveBatches,
    int TotalEnrollments);
