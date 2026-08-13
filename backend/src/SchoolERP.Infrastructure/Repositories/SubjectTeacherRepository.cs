using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.SubjectTeacher.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for the <see cref="SubjectTeacher"/> join entity.
/// Works only with the entity; never returns DTOs.
/// </summary>
public class SubjectTeacherRepository : ISubjectTeacherRepository
{
    private readonly SchoolERPDbContext _context;
    private readonly DbSet<SubjectTeacher> _dbSet;

    public SubjectTeacherRepository(SchoolERPDbContext context)
    {
        _context = context;
        _dbSet = context.Set<SubjectTeacher>();
    }

    public async Task<SubjectTeacher?> GetAsync(int subjectId, int teacherId, CancellationToken cancellationToken = default)
        => await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SubjectId == subjectId && x.TeacherId == teacherId, cancellationToken);

    public async Task<IReadOnlyList<SubjectTeacher>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(int subjectId, int teacherId, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().AnyAsync(x => x.SubjectId == subjectId && x.TeacherId == teacherId, cancellationToken);

    public async Task<SubjectTeacher> AddAsync(SubjectTeacher entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public void Remove(SubjectTeacher entity)
    {
        _dbSet.Remove(entity);
    }
}
