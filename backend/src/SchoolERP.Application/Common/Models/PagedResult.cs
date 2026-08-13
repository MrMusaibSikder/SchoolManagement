namespace SchoolERP.Application.Common.Models;

/// <summary>
/// Generic, read-only paged result wrapper returned by every paginated Service
/// method in the project. Keeps paging metadata (page number/size/total count)
/// alongside the requested page of items.
/// </summary>
/// <typeparam name="T">The DTO type contained in the page.</typeparam>
public class PagedResult<T>
{
    /// <summary>The items contained in the current page.</summary>
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

    /// <summary>Total number of records matching the query, across all pages.</summary>
    public int TotalCount { get; set; }

    /// <summary>The 1-based page number returned.</summary>
    public int PageNumber { get; set; }

    /// <summary>The page size used to produce this result.</summary>
    public int PageSize { get; set; }

    /// <summary>Total number of pages available for the given page size.</summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>Whether a page before this one exists.</summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>Whether a page after this one exists.</summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
