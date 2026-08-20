using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Enrollments.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Application.Features.Enrollments;

public class EnrollmentService : IEnrollmentService
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    /// <summary>
    /// Statuses that occupy a seat against Batch.Capacity — Cancelled and
    /// Completed don't (a completed student has finished, a cancelled one
    /// never took/gave up the seat).
    /// </summary>
    private static readonly EnrollmentStatus[] SeatOccupyingStatuses = { EnrollmentStatus.Pending, EnrollmentStatus.Active };

    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IBatchRepository _batchRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        IBatchRepository batchRepository,
        IUnitOfWork unitOfWork)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _batchRepository = batchRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<EnrollmentResponse>> SearchAsync(
        Guid? studentId,
        Guid? batchId,
        EnrollmentStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var normalizedPage = Math.Max(page, 1);
        var normalizedPageSize = Math.Clamp(pageSize <= 0 ? DefaultPageSize : pageSize, 1, MaxPageSize);

        var (items, totalCount) = await _enrollmentRepository.SearchAsync(
            studentId, batchId, status, normalizedPage, normalizedPageSize, cancellationToken);

        var responses = items.Select(ToResponse).ToList();

        return new PagedResult<EnrollmentResponse>(responses, totalCount, normalizedPage, normalizedPageSize);
    }

    public async Task<EnrollmentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var enrollment = await RequireEnrollmentAsync(id, cancellationToken);
        return ToResponse(enrollment);
    }

    public async Task<EnrollmentResponse> CreateAsync(CreateEnrollmentRequest request, CancellationToken cancellationToken = default)
    {
        // Every check below throws a friendly 400/404 instead of letting
        // the DB's unique (StudentId, BatchId) index (see
        // EnrollmentConfiguration) or an unenforced business rule (seat
        // capacity) produce a confusing result.
        var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken)
            ?? throw new NotFoundException("Student", request.StudentId);

        if (!student.IsActive)
        {
            throw new ValidationException($"Student '{student.FirstName} {student.LastName}' is not active.");
        }

        var batch = await _batchRepository.GetByIdAsync(request.BatchId, cancellationToken)
            ?? throw new NotFoundException("Batch", request.BatchId);

        if (!batch.IsActive)
        {
            throw new ValidationException($"Batch '{batch.Name}' is not active — reactivate it before enrolling new students.");
        }

        if (await _enrollmentRepository.ExistsForStudentAndBatchAsync(student.Id, batch.Id, cancellationToken))
        {
            throw new ValidationException("This student is already enrolled in this batch.");
        }

        if (batch.Capacity.HasValue)
        {
            var occupiedSeats = await _enrollmentRepository.CountForBatchByStatusesAsync(batch.Id, SeatOccupyingStatuses, cancellationToken);

            if (occupiedSeats >= batch.Capacity.Value)
            {
                throw new ValidationException($"Batch '{batch.Name}' is at full capacity ({batch.Capacity.Value}).");
            }
        }

        var enrollment = Enrollment.Create(student.Id, batch.Id);

        await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(enrollment);
    }

    public async Task<EnrollmentResponse> ApproveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var enrollment = await RequireEnrollmentAsync(id, cancellationToken);
        // Enrollment.Approve() throws a domain DomainException (mapped to
        // 400 by GlobalExceptionHandler) if it isn't currently Pending —
        // no need to duplicate that state-machine check here.
        enrollment.Approve();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(enrollment);
    }

    public async Task<EnrollmentResponse> CompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var enrollment = await RequireEnrollmentAsync(id, cancellationToken);
        enrollment.Complete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(enrollment);
    }

    public async Task<EnrollmentResponse> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var enrollment = await RequireEnrollmentAsync(id, cancellationToken);
        enrollment.Cancel();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(enrollment);
    }

    private async Task<Enrollment> RequireEnrollmentAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _enrollmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Enrollment", id);
    }

    private static EnrollmentResponse ToResponse(Enrollment enrollment) => new(
        enrollment.Id,
        enrollment.StudentId,
        enrollment.BatchId,
        enrollment.EnrollmentDate,
        enrollment.Status.ToString(),
        enrollment.CreatedAt,
        enrollment.UpdatedAt);
}
