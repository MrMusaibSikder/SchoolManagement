using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.AcademicYear.DTOs;
using SchoolERP.Application.Features.AcademicYear.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for AcademicYear records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class AcademicYearService : IAcademicYearService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AcademicYearService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AcademicYearDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.AcademicYearRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<AcademicYearDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<AcademicYearDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.AcademicYearRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<AcademicYearDto>(entity);
    }

    /// <inheritdoc />
    public async Task<AcademicYearDto> CreateAsync(CreateAcademicYearDto request, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<AcademicYear>(request);

        await _unitOfWork.AcademicYearRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AcademicYearDto>(entity);
    }

    /// <inheritdoc />
    public async Task<AcademicYearDto> UpdateAsync(int id, UpdateAcademicYearDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.AcademicYearRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(AcademicYear), id);

        _mapper.Map(request, entity);

        _unitOfWork.AcademicYearRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AcademicYearDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.AcademicYearRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(AcademicYear), id);

        _unitOfWork.AcademicYearRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

}
