using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Designation.DTOs;
using SchoolERP.Application.Features.Designation.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for Designation records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class DesignationService : IDesignationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DesignationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DesignationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.DesignationRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<DesignationDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<DesignationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.DesignationRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<DesignationDto>(entity);
    }

    /// <inheritdoc />
    public async Task<DesignationDto> CreateAsync(CreateDesignationDto request, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<Designation>(request);

        await _unitOfWork.DesignationRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DesignationDto>(entity);
    }

    /// <inheritdoc />
    public async Task<DesignationDto> UpdateAsync(int id, UpdateDesignationDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.DesignationRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Designation), id);

        _mapper.Map(request, entity);

        _unitOfWork.DesignationRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DesignationDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.DesignationRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Designation), id);

        _unitOfWork.DesignationRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
