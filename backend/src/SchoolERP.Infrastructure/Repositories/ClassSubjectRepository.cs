using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.ClassSubject.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for the <see cref="ClassSubject"/> join entity.
/// Works only with the entity; never returns DTOs.
/// </summary>
public class ClassSubjectRepository : IClassSubjectRepository
{
    private readonly SchoolERPDbContext _context;
    private readonly DbSet<ClassSubject> _dbSet;

    public ClassSubjectRepository(SchoolERPDbContext context)
    {
        _context = context;
        _dbSet = context.Set<ClassSubject>();
    }

    public async Task<ClassSubject?> GetAsync(int classId, int subjectId, CancellationToken cancellationToken = default)
        => await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ClassId == classId && x.SubjectId == subjectId, cancellationToken);

    public async Task<IReadOnlyList<ClassSubject>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(int classId, int subjectId, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().AnyAsync(x => x.ClassId == classId && x.SubjectId == subjectId, cancellationToken);

    public async Task<ClassSubject> AddAsync(ClassSubject entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public void Remove(ClassSubject entity)
    {
        _dbSet.Remove(entity);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<int>> GetOptionalSubjectIdsAsync(int classId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ClassId == classId && x.IsOptional)
            .Select(x => x.SubjectId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task SetOptionalAsync(int classId, int subjectId, bool isOptional, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(x => x.ClassId == classId && x.SubjectId == subjectId, cancellationToken);

        if (entity is not null)
        {
            entity.IsOptional = isOptional;
        }
    }
}
