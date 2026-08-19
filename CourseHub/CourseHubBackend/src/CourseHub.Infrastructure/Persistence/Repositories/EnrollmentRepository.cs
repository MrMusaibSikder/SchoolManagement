using CourseHub.Application.Common.Interfaces;
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
}
