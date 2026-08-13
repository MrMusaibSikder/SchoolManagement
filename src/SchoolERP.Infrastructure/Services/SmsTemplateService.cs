using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.SmsTemplate.DTOs;
using SchoolERP.Application.Features.SmsTemplate.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for SmsTemplate records. Calls the repository (via the Unit of Work),
/// applies business rules (unique name, placeholder rendering), and maps entities
/// to/from DTOs using AutoMapper.
/// </summary>
public class SmsTemplateService : ISmsTemplateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPlaceholderReplacementService _placeholderReplacementService;

    public SmsTemplateService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPlaceholderReplacementService placeholderReplacementService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _placeholderReplacementService = placeholderReplacementService;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SmsTemplateDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.SmsTemplateRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SmsTemplateDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<PagedResult<SmsTemplateDto>> GetPagedAsync(SmsTemplateQueryDto query, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _unitOfWork.SmsTemplateRepository.GetPagedAsync(
            query.SearchTerm,
            query.IsActive,
            query.PageNumber,
            query.PageSize,
            query.SortBy,
            query.SortDescending,
            cancellationToken);

        return new PagedResult<SmsTemplateDto>
        {
            Items = _mapper.Map<IReadOnlyList<SmsTemplateDto>>(items),
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    /// <inheritdoc />
    public async Task<SmsTemplateDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SmsTemplateRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<SmsTemplateDto>(entity);
    }

    /// <inheritdoc />
    public async Task<SmsTemplateDto> CreateAsync(CreateSmsTemplateDto request, CancellationToken cancellationToken = default)
    {
        await EnsureNameIsUniqueAsync(request.Name, excludeId: null, cancellationToken);

        var entity = _mapper.Map<SmsTemplate>(request);

        await _unitOfWork.SmsTemplateRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SmsTemplateDto>(entity);
    }

    /// <inheritdoc />
    public async Task<SmsTemplateDto> UpdateAsync(int id, UpdateSmsTemplateDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SmsTemplateRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(SmsTemplate), id);

        await EnsureNameIsUniqueAsync(request.Name, excludeId: id, cancellationToken);

        _mapper.Map(request, entity);

        _unitOfWork.SmsTemplateRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SmsTemplateDto>(entity);
    }

    /// <inheritdoc />
    public async Task<SmsTemplateDto> ActivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SmsTemplateRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(SmsTemplate), id);

        entity.IsActive = true;

        _unitOfWork.SmsTemplateRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SmsTemplateDto>(entity);
    }

    /// <inheritdoc />
    public async Task<SmsTemplateDto> DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SmsTemplateRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(SmsTemplate), id);

        entity.IsActive = false;

        _unitOfWork.SmsTemplateRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SmsTemplateDto>(entity);
    }

    /// <inheritdoc />
    public async Task<RenderedSmsTemplateDto> RenderAsync(int id, RenderSmsTemplateDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SmsTemplateRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(SmsTemplate), id);

        var rendered = _placeholderReplacementService.Replace(entity.Message, request.Data);

        return new RenderedSmsTemplateDto
        {
            TemplateId = entity.Id,
            TemplateName = entity.Name,
            RawMessage = entity.Message,
            RenderedMessage = rendered
        };
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SmsTemplateRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(SmsTemplate), id);

        _unitOfWork.SmsTemplateRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Ensures no other active template already uses the given name
    /// (case-insensitive). Throws <see cref="BadRequestException"/> if it does.
    /// </summary>
    private async Task EnsureNameIsUniqueAsync(string name, int? excludeId, CancellationToken cancellationToken)
    {
        var existing = await _unitOfWork.SmsTemplateRepository.GetByNameAsync(name, cancellationToken);

        if (existing is not null && existing.Id != excludeId)
        {
            throw new BadRequestException($"A template named '{name}' already exists.");
        }
    }
}
