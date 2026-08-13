namespace SchoolERP.Application.Common.Models;

/// <summary>
/// Base class for paginated, searchable, sortable query DTOs. Every module-level
/// query DTO (e.g. <c>SmsTemplateQueryDto</c>, <c>NoticeQueryDto</c>) inherits
/// this to stay consistent across the project.
/// </summary>
public abstract class PagedQueryDto
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>1-based page number to return. Defaults to 1.</summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>Number of records per page. Capped at 100 to protect the database.</summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>Free-text search term. Interpreted per-module (name, title, message, etc.).</summary>
    public string? SearchTerm { get; set; }

    /// <summary>Name of the column/property to sort by. Interpreted per-module.</summary>
    public string? SortBy { get; set; }

    /// <summary>Whether to sort descending instead of the default ascending order.</summary>
    public bool SortDescending { get; set; }
}
