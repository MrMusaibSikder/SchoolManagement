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
    public class SubmissionRepository : ISubmissionRepository
    {
        private readonly CourseHubDbContext _dbContext;

        public SubmissionRepository(CourseHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<AssignmentSubmission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _dbContext.AssignmentSubmissions.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public Task<AssignmentSubmission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId, CancellationToken cancellationToken = default)
        {
            return _dbContext.AssignmentSubmissions
                .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId, cancellationToken);
        }

        public async Task<IReadOnlyList<AssignmentSubmission>> GetByAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.AssignmentSubmissions
                .Where(s => s.AssignmentId == assignmentId)
                .OrderBy(s => s.SubmittedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<AssignmentSubmission> Items, int TotalCount)> GetByStudentAsync(
            Guid studentId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.AssignmentSubmissions.Where(s => s.StudentId == studentId);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(s => s.SubmittedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task AddAsync(AssignmentSubmission submission, CancellationToken cancellationToken = default)
        {
            await _dbContext.AssignmentSubmissions.AddAsync(submission, cancellationToken);
        }
    }
}
