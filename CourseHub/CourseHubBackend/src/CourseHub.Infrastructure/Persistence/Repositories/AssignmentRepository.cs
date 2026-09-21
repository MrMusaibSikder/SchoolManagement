using CourseHub.Application.Common.Interfaces;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Infrastructure.Persistence.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly CourseHubDbContext _dbContext;

        public AssignmentRepository(CourseHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _dbContext.Assignments.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<(IReadOnlyList<Assignment> Items, int TotalCount)> SearchAsync(
            Guid? courseId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Assignments.AsQueryable();

            if (courseId.HasValue)
            {
                query = query.Where(a => a.CourseId == courseId.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(a => a.DueDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<IReadOnlyList<Assignment>> GetActiveByCourseIdsAsync(IReadOnlyList<Guid> courseIds, CancellationToken cancellationToken = default)
        {
            if (courseIds.Count == 0)
            {
                return Array.Empty<Assignment>();
            }

            return await _dbContext.Assignments
                .Where(a => a.IsActive && courseIds.Contains(a.CourseId))
                .OrderByDescending(a => a.DueDate)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default)
        {
            await _dbContext.Assignments.AddAsync(assignment, cancellationToken);
        }
    }
}
