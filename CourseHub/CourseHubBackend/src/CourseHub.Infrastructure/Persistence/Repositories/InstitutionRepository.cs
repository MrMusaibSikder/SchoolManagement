using CourseHub.Application.Common.Interfaces;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Persistence.Repositories;

public class InstitutionRepository : IInstitutionRepository
{
    private readonly CourseHubDbContext _dbContext;

    public InstitutionRepository(CourseHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Institution?> GetAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Institutions.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Institution institution, CancellationToken cancellationToken = default)
    {
        await _dbContext.Institutions.AddAsync(institution, cancellationToken);
    }
}
