using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Section.DTOs;
using SchoolERP.Application.Features.Section.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for Section records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class SectionService : ISectionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SectionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SectionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.SectionRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SectionDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<SectionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SectionRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<SectionDto>(entity);
    }

    /// <inheritdoc />
    public async Task<SectionDto> CreateAsync(CreateSectionDto request, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<Section>(request);

        await _unitOfWork.SectionRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SectionDto>(entity);
    }

    /// <inheritdoc />
    public async Task<SectionDto> UpdateAsync(int id, UpdateSectionDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SectionRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Section), id);

        _mapper.Map(request, entity);

        _unitOfWork.SectionRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SectionDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SectionRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Section), id);

        _unitOfWork.SectionRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
