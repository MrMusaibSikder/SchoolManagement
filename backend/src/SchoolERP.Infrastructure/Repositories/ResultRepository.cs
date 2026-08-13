using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.Result.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="Result"/> (mark entry)
/// entities. Works only with the <see cref="Result"/> entity; never returns
/// DTOs. Contains database queries only — every business rule (teacher
/// assignment, exam-status gating, lock workflow) lives in <c>ResultService</c>.
/// </summary>
public class ResultRepository : GenericRepository<Result>, IResultRepository
{
    public ResultRepository(SchoolERPDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Result?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result?> GetByStudentAndScheduleAsync(int studentId, int examScheduleId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(x =>
                !x.IsDeleted &&
                x.StudentId == studentId &&
                x.ExamScheduleId == examScheduleId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Result>> GetByExamScheduleAsync(int examScheduleId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .Where(x => !x.IsDeleted && x.ExamScheduleId == examScheduleId)
            .OrderBy(x => x.Student!.RollNo)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Result>> GetByStudentAndExamAsync(int studentId, int examId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .Where(x =>
                !x.IsDeleted &&
                x.StudentId == studentId &&
                x.ExamSchedule!.ExamId == examId)
            .OrderBy(x => x.ExamSchedule!.Subject!.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Result>> GetByClassAndExamAsync(int classId, int examId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .Where(x =>
                !x.IsDeleted &&
                x.ExamSchedule!.ExamId == examId &&
                x.ExamSchedule!.ClassId == classId)
            .OrderBy(x => x.Student!.RollNo)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Result>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .Where(x => !x.IsDeleted && x.StudentId == studentId)
            .OrderBy(x => x.ExamSchedule!.ExamId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Result>> GetByExamAsync(int examId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .Where(x => !x.IsDeleted && x.ExamSchedule!.ExamId == examId)
            .OrderBy(x => x.Student!.ClassId)
            .ThenBy(x => x.Student!.RollNo)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountSubmittedByScheduleAsync(int examScheduleId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .CountAsync(x =>
                !x.IsDeleted &&
                x.ExamScheduleId == examScheduleId &&
                x.EntryStatus == MarkEntryStatus.Submitted,
                cancellationToken);
    }

    /// <summary>Shared base query eagerly loading Student, ExamSchedule, Exam and Subject.</summary>
    private IQueryable<Result> WithDetailsQuery()
    {
        return DbSet
            .AsNoTracking()
            .Include(x => x.Student)
            .Include(x => x.ExamSchedule!).ThenInclude(s => s!.Exam)
            .Include(x => x.ExamSchedule!).ThenInclude(s => s!.Subject)
            .Include(x => x.EnteredByTeacher!).ThenInclude(t => t!.Employee);
    }
}
