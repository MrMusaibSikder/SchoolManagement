using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.ExamSchedule.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="ExamSchedule"/> entities.
/// Works only with the <see cref="ExamSchedule"/> entity; never returns DTOs.
/// Contains database queries only — every business rule (Exam-status gating,
/// duplicate handling, etc.) lives in <c>ExamScheduleService</c>.
/// </summary>
public class ExamScheduleRepository : GenericRepository<ExamSchedule>, IExamScheduleRepository
{
    public ExamScheduleRepository(SchoolERPDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ExamSchedule?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamSchedule>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.ExamDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamSchedule>> GetSchedulesByExamAsync(int examId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .Where(x => !x.IsDeleted && x.ExamId == examId)
            .OrderBy(x => x.ExamDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamSchedule>> GetSchedulesByClassAsync(int classId, int? examId, CancellationToken cancellationToken = default)
    {
        var query = WithDetailsQuery().Where(x => !x.IsDeleted && x.ClassId == classId);

        if (examId.HasValue)
        {
            query = query.Where(x => x.ExamId == examId.Value);
        }

        return await query
            .OrderBy(x => x.ExamDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamSchedule>> GetSchedulesByTeacherAsync(int teacherId, int? examId, CancellationToken cancellationToken = default)
    {
        var subjectIds = Context.Set<SubjectTeacher>()
            .AsNoTracking()
            .Where(x => x.TeacherId == teacherId)
            .Select(x => x.SubjectId);

        var query = WithDetailsQuery().Where(x => !x.IsDeleted && subjectIds.Contains(x.SubjectId));

        if (examId.HasValue)
        {
            query = query.Where(x => x.ExamId == examId.Value);
        }

        return await query
            .OrderBy(x => x.ExamDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamSchedule>> GetExamCalendarAsync(DateTime fromDate, DateTime toDate, int? classId, CancellationToken cancellationToken = default)
    {
        var from = fromDate.Date;
        var to = toDate.Date;

        var query = WithDetailsQuery()
            .Where(x => !x.IsDeleted && x.ExamDate.Date >= from && x.ExamDate.Date <= to);

        if (classId.HasValue)
        {
            query = query.Where(x => x.ClassId == classId.Value);
        }

        return await query
            .OrderBy(x => x.ExamDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DuplicateScheduleExistsAsync(int examId, int classId, int subjectId, int? excludeId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(x =>
                !x.IsDeleted &&
                x.ExamId == examId &&
                x.ClassId == classId &&
                x.SubjectId == subjectId &&
                (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DateAlreadyScheduledAsync(int examId, int classId, DateTime examDate, int? excludeId, CancellationToken cancellationToken = default)
    {
        var date = examDate.Date;

        return await DbSet
            .AsNoTracking()
            .AnyAsync(x =>
                !x.IsDeleted &&
                x.ExamId == examId &&
                x.ClassId == classId &&
                x.ExamDate.Date == date &&
                (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
    }

    /// <summary>Shared base query eagerly loading Exam, SchoolClass and Subject.</summary>
    private IQueryable<ExamSchedule> WithDetailsQuery()
    {
        return DbSet
            .AsNoTracking()
            .Include(x => x.Exam)
            .Include(x => x.SchoolClass)
            .Include(x => x.Subject);
    }
}
