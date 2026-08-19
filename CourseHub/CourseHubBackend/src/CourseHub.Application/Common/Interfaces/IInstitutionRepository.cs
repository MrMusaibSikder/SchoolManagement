using CourseHub.Domain.Entities;

namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// CourseHub is single-institute: there is conceptually exactly one
/// Institution row, used to power the public landing page. This
/// abstraction reflects that — "get the one institute" rather than a
/// paged/filtered query set.
/// </summary>
public interface IInstitutionRepository
{
    Task<Institution?> GetAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Institution institution, CancellationToken cancellationToken = default);
}
