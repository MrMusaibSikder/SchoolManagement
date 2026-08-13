using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Features.SmsTemplate.Interfaces;

/// <summary>
/// Repository contract for <see cref="SchoolERP.Domain.Entities.SmsTemplate"/> entities.
/// Extends the generic repository with SmsTemplate-specific data access members.
/// Contains database operations only.
/// </summary>
public interface ISmsTemplateRepository : IGenericRepository<SchoolERP.Domain.Entities.SmsTemplate>
{
    /// <summary>Finds a template by its exact (case-insensitive) name, or null if none exists.</summary>
    Task<SchoolERP.Domain.Entities.SmsTemplate?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a search-filtered, sorted page of templates, along with the
    /// total number of matching records (before paging).
    /// </summary>
    Task<(IReadOnlyList<SchoolERP.Domain.Entities.SmsTemplate> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        bool? isActive,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool sortDescending,
        CancellationToken cancellationToken = default);
}
