using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.StudentGuardian.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for the <see cref="StudentGuardian"/> join entity.
/// Works only with the entity; never returns DTOs.
/// </summary>
public class StudentGuardianRepository : IStudentGuardianRepository
{
    private readonly SchoolERPDbContext _context;
    private readonly DbSet<StudentGuardian> _dbSet;

    public StudentGuardianRepository(SchoolERPDbContext context)
    {
        _context = context;
        _dbSet = context.Set<StudentGuardian>();
    }

    public async Task<StudentGuardian?> GetAsync(int studentId, int guardianId, CancellationToken cancellationToken = default)
        => await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.StudentId == studentId && x.GuardianId == guardianId, cancellationToken);

    public async Task<IReadOnlyList<StudentGuardian>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(int studentId, int guardianId, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().AnyAsync(x => x.StudentId == studentId && x.GuardianId == guardianId, cancellationToken);

    public async Task<StudentGuardian> AddAsync(StudentGuardian entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public void Remove(StudentGuardian entity)
    {
        _dbSet.Remove(entity);
    }
}
