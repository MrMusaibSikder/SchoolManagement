using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Enrollments.Dtos;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Application.Features.Me;

public class MeService : IMeService
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IStudentRepository _studentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public MeService(IStudentRepository studentRepository, IEnrollmentRepository enrollmentRepository)
    {
        _studentRepository = studentRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<PagedResult<EnrollmentResponse>> GetMyEnrollmentsAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("You don't have a student profile yet — ask an administrator to create one for you.");

        var normalizedPage = Math.Max(page, 1);
        var normalizedPageSize = Math.Clamp(pageSize <= 0 ? DefaultPageSize : pageSize, 1, MaxPageSize);

        var (items, totalCount) = await _enrollmentRepository.SearchAsync(
            student.Id,
            null,
            null,
            normalizedPage,
            normalizedPageSize,
            cancellationToken);

        var responses = items
            .Select(e => new EnrollmentResponse(e.Id, e.StudentId, e.BatchId, e.EnrollmentDate, e.Status.ToString(), e.CreatedAt, e.UpdatedAt))
            .ToList();

        return new PagedResult<EnrollmentResponse>(responses, totalCount, normalizedPage, normalizedPageSize);
    }
}
