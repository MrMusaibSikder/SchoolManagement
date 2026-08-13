using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.SchoolClass.DTOs;
using SchoolERP.Application.Features.SchoolClass.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for SchoolClass records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class SchoolClassService : ISchoolClassService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SchoolClassService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SchoolClassDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.SchoolClassRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SchoolClassDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<SchoolClassDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SchoolClassRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<SchoolClassDto>(entity);
    }

    /// <inheritdoc />
    public async Task<SchoolClassDto> CreateAsync(CreateSchoolClassDto request, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<SchoolClass>(request);

        await _unitOfWork.SchoolClassRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SchoolClassDto>(entity);
    }

    /// <inheritdoc />
    public async Task<SchoolClassDto> UpdateAsync(int id, UpdateSchoolClassDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SchoolClassRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(SchoolClass), id);

        _mapper.Map(request, entity);

        _unitOfWork.SchoolClassRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SchoolClassDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SchoolClassRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(SchoolClass), id);

        _unitOfWork.SchoolClassRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
