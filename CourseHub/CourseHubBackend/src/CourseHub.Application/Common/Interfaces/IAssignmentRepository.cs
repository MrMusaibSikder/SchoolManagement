using CourseHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Application.Common.Interfaces
{
    public interface IAssignmentRepository
    {
        Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Admin/Teacher listing: optional course filter, paged. Returns
        /// every assignment regardless of IsActive — this is the management
        /// screen, not a public/student-facing one.
        /// </summary>
        Task<(IReadOnlyList<Assignment> Items, int TotalCount)> SearchAsync(
            Guid? courseId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Active assignments belonging to any of the given courses — this is
        /// what MeService.GetMyAssignmentsAsync uses: resolve a student's
        /// enrolled courses first, then fetch assignments for exactly those.
        /// </summary>
        Task<IReadOnlyList<Assignment>> GetActiveByCourseIdsAsync(IReadOnlyList<Guid> courseIds, CancellationToken cancellationToken = default);

        Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default);
    }
}
