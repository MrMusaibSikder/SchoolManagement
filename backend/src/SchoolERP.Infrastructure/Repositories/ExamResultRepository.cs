using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.ExamResult.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="ExamResult"/> entities.
/// Works only with the <see cref="ExamResult"/> entity; never returns DTOs.
/// </summary>
public class ExamResultRepository : GenericRepository<ExamResult>, IExamResultRepository
{
    public ExamResultRepository(SchoolERPDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ExamResult?> GetByStudentAndExamAsync(int studentId, int examId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .FirstOrDefaultAsync(x => x.StudentId == studentId && x.ExamId == examId && !x.IsDeleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamResult>> GetByExamAsync(int examId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .Where(x => x.ExamId == examId && !x.IsDeleted)
            .OrderBy(x => x.MeritPosition ?? int.MaxValue)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamResult>> GetByExamAndClassAsync(int examId, int classId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .Where(x => x.ExamId == examId && !x.IsDeleted && x.Student!.ClassId == classId)
            .OrderBy(x => x.ClassPosition ?? int.MaxValue)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamResult>> GetByExamAndSectionAsync(int examId, int sectionId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .Where(x => x.ExamId == examId && !x.IsDeleted && x.Student!.SectionId == sectionId)
            .OrderBy(x => x.SectionPosition ?? int.MaxValue)
            .ToListAsync(cancellationToken);
    }

    /// <summary>Shared base query eagerly loading Student, SchoolClass and Section.</summary>
    private IQueryable<ExamResult> WithDetailsQuery()
    {
        return DbSet
            .AsNoTracking()
            .Include(x => x.Student!).ThenInclude(s => s.SchoolClass)
            .Include(x => x.Student!).ThenInclude(s => s.Section)
            .Include(x => x.Exam);
    }
}
