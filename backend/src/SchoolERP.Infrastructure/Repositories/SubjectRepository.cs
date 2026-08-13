using SchoolERP.Application.Features.Subject.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="Subject"/> entities.
/// Works only with the <see cref="Subject"/> entity; never returns DTOs.
/// </summary>
public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(SchoolERPDbContext context) : base(context)
    {
    }
}
//public async Task<IReadOnlyList<Subject>> SearchAsync(
//   string? search,
//   string? sortBy,
//   bool descending,
//   int page,
//   int pageSize,
//   CancellationToken cancellationToken = default)
//{
//    IQueryable<Subject> query = Context.Subjects
//        .AsNoTracking()
//        .Where(x => !x.IsDeleted);

//    // Search
//    if (!string.IsNullOrWhiteSpace(search))
//    {
//        search = search.Trim();

//        query = query.Where(x =>
//            x.Name.Contains(search) ||
//            x.Code.Contains(search));
//    }

//    // Sorting
//    query = (sortBy?.ToLower()) switch
//    {
//        "code" => descending
//            ? query.OrderByDescending(x => x.Code)
//            : query.OrderBy(x => x.Code),

//        "fullmarks" => descending
//            ? query.OrderByDescending(x => x.FullMarks)
//            : query.OrderBy(x => x.FullMarks),

//        "passmarks" => descending
//            ? query.OrderByDescending(x => x.PassMarks)
//            : query.OrderBy(x => x.PassMarks),

//        "createdat" => descending
//            ? query.OrderByDescending(x => x.CreatedAt)
//            : query.OrderBy(x => x.CreatedAt),

//        _ => descending
//            ? query.OrderByDescending(x => x.Name)
//            : query.OrderBy(x => x.Name)
//    };

//    // Pagination
//    page = page <= 0 ? 1 : page;
//    pageSize = pageSize <= 0 ? 20 : pageSize;

//    return await query
//        .Skip((page - 1) * pageSize)
//        .Take(pageSize)
//        .ToListAsync(cancellationToken);
//}