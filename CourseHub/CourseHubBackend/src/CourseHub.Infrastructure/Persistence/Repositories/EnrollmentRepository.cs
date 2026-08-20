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

    public async Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        await _dbContext.Enrollments.AddAsync(enrollment, cancellationToken);
    }
}
