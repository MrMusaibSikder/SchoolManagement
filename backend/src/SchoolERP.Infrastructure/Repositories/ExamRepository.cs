using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.Exam.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="Exam"/> entities.
/// Works only with the <see cref="Exam"/> entity; never returns DTOs.
/// Contains database queries only — every business rule (lifecycle
/// transitions, dashboard/report composition, etc.) lives in <c>ExamService</c>.
/// </summary>
public class ExamRepository : GenericRepository<Exam>, IExamRepository
{
    public ExamRepository(SchoolERPDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Exam?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(x => x.ExamType)
            .Include(x => x.AcademicYear)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Exam>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(x => x.ExamType)
            .Include(x => x.AcademicYear)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Exam?> GetExamWithSchedulesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await GetWithSchedulesQuery()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Exam?> GetExamWithStatisticsAsync(int id, CancellationToken cancellationToken = default)
    {
        // Statistics are computed by the Service from the same schedule graph
        // used for "with schedules" reads, so this shares the same query.
        return await GetWithSchedulesQuery()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Exam>> GetUpcomingExamsAsync(DateTime asOfDate, int count, CancellationToken cancellationToken = default)
    {
        var date = asOfDate.Date;

        return await GetWithSchedulesQuery()
            .Where(x =>
                x.Status == ExamStatus.Published &&
                x.ExamSchedules.Any(s => !s.IsDeleted && s.ExamDate.Date >= date))
            .OrderBy(x => x.ExamSchedules
                .Where(s => !s.IsDeleted && s.ExamDate.Date >= date)
                .Min(s => s.ExamDate))
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Exam>> GetCompletedExamsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(x => x.ExamType)
            .Include(x => x.AcademicYear)
            .Where(x => !x.IsDeleted && x.Status == ExamStatus.Completed)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Exam>> GetRecentExamsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await GetWithSchedulesQuery()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountByStatusAsync(ExamStatus? status, CancellationToken cancellationToken = default)
    {
        IQueryable<Exam> query = DbSet.AsNoTracking().Where(x => !x.IsDeleted);

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return await query.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ExamStatus?> GetExamStatusAsync(int examId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => x.Id == examId && !x.IsDeleted)
            .Select(x => (ExamStatus?)x.Status)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExamExistsAsync(int examId, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(examId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DuplicateExamExistsAsync(
        string name,
        int academicYearId,
        int examTypeId,
        int? excludeId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(x =>
                !x.IsDeleted &&
                x.AcademicYearId == academicYearId &&
                x.ExamTypeId == examTypeId &&
                x.Name.ToLower() == name.ToLower() &&
                (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
    }

    /// <summary>Shared base query for reads that need the full exam + schedule + subject/class graph.</summary>
    private IQueryable<Exam> GetWithSchedulesQuery()
    {
        return DbSet
            .AsNoTracking()
            .Include(x => x.ExamType)
            .Include(x => x.AcademicYear)
            .Include(x => x.ExamSchedules.Where(s => !s.IsDeleted))
                .ThenInclude(s => s.Subject)
            .Include(x => x.ExamSchedules.Where(s => !s.IsDeleted))
                .ThenInclude(s => s.SchoolClass);
    }
}
