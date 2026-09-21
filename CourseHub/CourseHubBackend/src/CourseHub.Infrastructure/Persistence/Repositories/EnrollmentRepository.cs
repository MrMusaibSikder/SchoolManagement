using CourseHub.Application.Common.Interfaces;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;
using CourseHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Persistence.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly CourseHubDbContext _dbContext;

    public EnrollmentRepository(CourseHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> CountActiveOrCompletedAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Enrollments.CountAsync(
            e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed,
            cancellationToken);
    }

    public Task<Enrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Enrollments.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public Task<bool> ExistsForStudentAndBatchAsync(Guid studentId, Guid batchId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Enrollments.AnyAsync(
            e => e.StudentId == studentId && e.BatchId == batchId,
            cancellationToken);
    }

    public Task<int> CountForBatchByStatusesAsync(Guid batchId, IReadOnlyList<EnrollmentStatus> statuses, CancellationToken cancellationToken = default)
    {
        return _dbContext.Enrollments.CountAsync(
            e => e.BatchId == batchId && statuses.Contains(e.Status),
            cancellationToken);
    }

    public async Task<(IReadOnlyList<Enrollment> Items, int TotalCount)> SearchAsync(
        Guid? studentId,
        Guid? batchId,
        EnrollmentStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Enrollments.AsQueryable();

        if (studentId.HasValue)
        {
            query = query.Where(e => e.StudentId == studentId.Value);
        }

        if (batchId.HasValue)
        {
            query = query.Where(e => e.BatchId == batchId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(e => e.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(e => e.EnrollmentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Enrollment>> GetActiveByBatchIdAsync(
    Guid batchId,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Enrollments
            .Where(e =>
                e.BatchId == batchId &&
                e.Status == EnrollmentStatus.Active)
            .OrderBy(e => e.StudentId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        await _dbContext.Enrollments.AddAsync(enrollment, cancellationToken);
    }

    public Task RemoveAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        _dbContext.Enrollments.Remove(enrollment);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Guid>> GetEnrolledStudentIdsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var query =
            from batch in _dbContext.Batches
            where batch.CourseId == courseId
            join enrollment in _dbContext.Enrollments on batch.Id equals enrollment.BatchId
            where enrollment.Status == EnrollmentStatus.Active || enrollment.Status == EnrollmentStatus.Completed
            select enrollment.StudentId;

        return await query.Distinct().ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsForStudentAndCourseAsync(
     Guid studentId,
     Guid courseId,
     CancellationToken cancellationToken = default)
    {
        var query =
            from batch in _dbContext.Batches
            where batch.CourseId == courseId
            join enrollment in _dbContext.Enrollments
                on batch.Id equals enrollment.BatchId
            where enrollment.StudentId == studentId
                  && (enrollment.Status == EnrollmentStatus.Active
                      || enrollment.Status == EnrollmentStatus.Completed)
            select enrollment;

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetEnrolledCourseIdsByStudentIdAsync(
     Guid studentId,
     CancellationToken cancellationToken = default)
    {
        return await (
            from enrollment in _dbContext.Enrollments
            join batch in _dbContext.Batches
                on enrollment.BatchId equals batch.Id
            where enrollment.StudentId == studentId
                  && (enrollment.Status == EnrollmentStatus.Active
                      || enrollment.Status == EnrollmentStatus.Completed)
            select batch.CourseId
        )
        .Distinct()
        .ToListAsync(cancellationToken);
    }
}
