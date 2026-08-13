using AutoMapper;
using Microsoft.AspNetCore.Http;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.Notice.DTOs;
using SchoolERP.Application.Features.Notice.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for Notice records. Calls the repository (via the Unit of
/// Work), applies business rules (unique title, publish date validation,
/// publish/unpublish/archive/restore workflow, attachment handling via
/// <see cref="IFileService"/>), and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class NoticeService : INoticeService
{
    private const string AttachmentFolder = "notices";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;

    public NoticeService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileService = fileService;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<NoticeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.NoticeRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<NoticeDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<PagedResult<NoticeDto>> GetPagedAsync(NoticeQueryDto query, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _unitOfWork.NoticeRepository.GetPagedAsync(
            query.SearchTerm,
            query.Audience,
            query.Priority,
            query.IsPublished,
            query.IsArchived,
            query.FromDate,
            query.ToDate,
            query.PageNumber,
            query.PageSize,
            query.SortBy,
            query.SortDescending,
            cancellationToken);

        return new PagedResult<NoticeDto>
        {
            Items = _mapper.Map<IReadOnlyList<NoticeDto>>(items),
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    /// <inheritdoc />
    public async Task<NoticeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.NoticeRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<NoticeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<NoticeDto> CreateAsync(CreateNoticeDto request, CancellationToken cancellationToken = default)
    {
        ValidateDateRange(request.PublishDate, request.ExpiryDate);
        await EnsureTitleIsUniqueAsync(request.Title, excludeId: null, cancellationToken);

        var entity = _mapper.Map<Notice>(request);

        if (request.AttachmentFile is not null)
        {
            entity.AttachmentPath = await UploadAsync(request.AttachmentFile, cancellationToken);
        }

        await _unitOfWork.NoticeRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NoticeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<NoticeDto> UpdateAsync(int id, UpdateNoticeDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.NoticeRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Notice), id);

        ValidateDateRange(request.PublishDate, request.ExpiryDate);
        await EnsureTitleIsUniqueAsync(request.Title, excludeId: id, cancellationToken);

        _mapper.Map(request, entity);

        if (request.RemoveAttachment && request.AttachmentFile is null)
        {
            await _fileService.DeleteAsync(entity.AttachmentPath);
            entity.AttachmentPath = null;
        }
        else if (request.AttachmentFile is not null)
        {
            entity.AttachmentPath = await _fileService.ReplaceAsync(
                request.AttachmentFile.OpenReadStream(),
                entity.AttachmentPath,
                request.AttachmentFile.FileName,
                AttachmentFolder,
                request.AttachmentFile.ContentType,
                request.AttachmentFile.Length,
                cancellationToken);
        }

        _unitOfWork.NoticeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NoticeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<NoticeDto> PublishAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetTrackedOrThrowAsync(id, cancellationToken);

        if (entity.IsArchived)
            throw new BadRequestException("An archived notice must be restored before it can be published.");

        entity.IsPublished = true;

        _unitOfWork.NoticeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NoticeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<NoticeDto> UnpublishAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetTrackedOrThrowAsync(id, cancellationToken);

        entity.IsPublished = false;

        _unitOfWork.NoticeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NoticeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<NoticeDto> ArchiveAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetTrackedOrThrowAsync(id, cancellationToken);

        entity.IsArchived = true;

        _unitOfWork.NoticeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NoticeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<NoticeDto> RestoreAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetTrackedOrThrowAsync(id, cancellationToken);

        entity.IsArchived = false;

        _unitOfWork.NoticeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NoticeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<NoticeDto> UploadAttachmentAsync(int id, IFormFile attachmentFile, CancellationToken cancellationToken = default)
    {
        if (attachmentFile is null || attachmentFile.Length == 0)
            throw new BadRequestException("An attachment file is required.");

        var entity = await GetTrackedOrThrowAsync(id, cancellationToken);

        entity.AttachmentPath = await _fileService.ReplaceAsync(
            attachmentFile.OpenReadStream(),
            entity.AttachmentPath,
            attachmentFile.FileName,
            AttachmentFolder,
            attachmentFile.ContentType,
            attachmentFile.Length,
            cancellationToken);

        _unitOfWork.NoticeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NoticeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<NoticeDto> RemoveAttachmentAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetTrackedOrThrowAsync(id, cancellationToken);

        if (!string.IsNullOrWhiteSpace(entity.AttachmentPath))
        {
            await _fileService.DeleteAsync(entity.AttachmentPath);
            entity.AttachmentPath = null;

            _unitOfWork.NoticeRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return _mapper.Map<NoticeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<NoticeDto>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.NoticeRepository.GetActiveAsync(DateTime.Today, cancellationToken);
        return _mapper.Map<IReadOnlyList<NoticeDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<NoticeDto>> GetUpcomingAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.NoticeRepository.GetUpcomingAsync(DateTime.Today, cancellationToken);
        return _mapper.Map<IReadOnlyList<NoticeDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<NoticeDto>> GetExpiredAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.NoticeRepository.GetExpiredAsync(DateTime.Today, cancellationToken);
        return _mapper.Map<IReadOnlyList<NoticeDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<NoticeDto>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.NoticeRepository.GetRecentAsync(count, cancellationToken);
        return _mapper.Map<IReadOnlyList<NoticeDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<NoticeDto>> GetHighPriorityAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.NoticeRepository.GetHighPriorityAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<NoticeDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<NoticeDashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;

        var total = await _unitOfWork.NoticeRepository.CountByStateAsync(null, null, cancellationToken);
        var published = await _unitOfWork.NoticeRepository.CountByStateAsync(true, false, cancellationToken);
        var draft = await _unitOfWork.NoticeRepository.CountByStateAsync(false, false, cancellationToken);
        var archived = await _unitOfWork.NoticeRepository.CountByStateAsync(null, true, cancellationToken);

        var active = await _unitOfWork.NoticeRepository.GetActiveAsync(today, cancellationToken);
        var upcoming = await _unitOfWork.NoticeRepository.GetUpcomingAsync(today, cancellationToken);
        var expired = await _unitOfWork.NoticeRepository.GetExpiredAsync(today, cancellationToken);
        var highPriority = await _unitOfWork.NoticeRepository.GetHighPriorityAsync(cancellationToken);

        return new NoticeDashboardSummaryDto
        {
            TotalNotices = total,
            PublishedNotices = published,
            DraftNotices = draft,
            ArchivedNotices = archived,
            ActiveNotices = active.Count,
            UpcomingNotices = upcoming.Count,
            ExpiredNotices = expired.Count,
            HighPriorityNotices = highPriority.Count
        };
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.NoticeRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Notice), id);

        _unitOfWork.NoticeRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Retrieves a tracked Notice entity or throws <see cref="NotFoundException"/>.</summary>
    private async Task<Notice> GetTrackedOrThrowAsync(int id, CancellationToken cancellationToken)
    {
        return await _unitOfWork.NoticeRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Notice), id);
    }

    /// <summary>Ensures the expiry date (when supplied) is not earlier than the publish date.</summary>
    private static void ValidateDateRange(DateTime publishDate, DateTime? expiryDate)
    {
        if (expiryDate.HasValue && expiryDate.Value.Date < publishDate.Date)
        {
            throw new BadRequestException("Expiry date cannot be earlier than the publish date.");
        }
    }

    /// <summary>Ensures no other (non-deleted) notice already uses the given title.</summary>
    private async Task EnsureTitleIsUniqueAsync(string title, int? excludeId, CancellationToken cancellationToken)
    {
        var exists = await _unitOfWork.NoticeRepository.TitleExistsAsync(title, excludeId, cancellationToken);

        if (exists)
        {
            throw new BadRequestException($"A notice titled '{title}' already exists.");
        }
    }

    /// <summary>Uploads a new attachment and returns its stored relative path.</summary>
    private async Task<string?> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        return await _fileService.UploadAsync(
            file.OpenReadStream(),
            file.FileName,
            AttachmentFolder,
            file.ContentType,
            file.Length,
            cancellationToken);
    }
}
