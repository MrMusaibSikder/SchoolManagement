namespace CourseHub.Application.Features.Courses.Dtos;

/// <summary>
/// TeacherId = null unassigns whichever teacher currently owns the
/// course — see CourseService.AssignTeacherAsync.
/// </summary>
public record AssignCourseTeacherRequest(Guid? TeacherId);
