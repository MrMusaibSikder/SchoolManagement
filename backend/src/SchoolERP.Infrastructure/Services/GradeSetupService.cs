using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.GradeSetup.DTOs;
using SchoolERP.Application.Features.GradeSetup.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for configurable grade bands. Calls the repository (via
/// the Unit of Work), enforces unique grade names and non-overlapping
/// percentage ranges per academic year, and maps entities to/from DTOs using
/// AutoMapper.
/// </summary>
public class GradeSetupService : IGradeSetupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GradeSetupService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GradeSetupDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.GradeSetupRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<GradeSetupDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<GradeSetupDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.GradeSetupRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<GradeSetupDto>(entity);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GradeSetupDto>> GetByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.GradeSetupRepository.GetByAcademicYearAsync(academicYearId, cancellationToken);
        return _mapper.Map<IReadOnlyList<GradeSetupDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<GradeSetupDto> CreateAsync(CreateGradeSetupDto request, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.AcademicYearRepository.ExistsAsync(request.AcademicYearId, cancellationToken))
        {
            throw new NotFoundException(nameof(AcademicYear), request.AcademicYearId);
        }

        await EnsureNameIsUniqueAsync(request.AcademicYearId, request.GradeName, excludeId: null, cancellationToken);
        await EnsureNoOverlapAsync(request.AcademicYearId, request.MinPercentage, request.MaxPercentage, excludeId: null, cancellationToken);

        var entity = _mapper.Map<Domain.Entities.GradeSetup>(request);

        await _unitOfWork.GradeSetupRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GradeSetupDto>(entity);
    }

    /// <inheritdoc />
    public async Task<GradeSetupDto> UpdateAsync(int id, UpdateGradeSetupDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.GradeSetupRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.GradeSetup), id);

        await EnsureNameIsUniqueAsync(entity.AcademicYearId, request.GradeName, excludeId: id, cancellationToken);
        await EnsureNoOverlapAsync(entity.AcademicYearId, request.MinPercentage, request.MaxPercentage, excludeId: id, cancellationToken);

        _mapper.Map(request, entity);

        _unitOfWork.GradeSetupRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GradeSetupDto>(entity);
    }

    /// <inheritdoc />
    public async Task<GradeSetupDto> ActivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.GradeSetupRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.GradeSetup), id);

        await EnsureNoOverlapAsync(entity.AcademicYearId, entity.MinPercentage, entity.MaxPercentage, excludeId: id, cancellationToken);

        entity.IsActive = true;
        _unitOfWork.GradeSetupRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GradeSetupDto>(entity);
    }

    /// <inheritdoc />
    public async Task<GradeSetupDto> DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.GradeSetupRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.GradeSetup), id);

        entity.IsActive = false;
        _unitOfWork.GradeSetupRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GradeSetupDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.GradeSetupRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.GradeSetup), id);

        _unitOfWork.GradeSetupRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Ensures no other (non-deleted) grade band already uses this name within the academic year.</summary>
    private async Task EnsureNameIsUniqueAsync(int academicYearId, string gradeName, int? excludeId, CancellationToken cancellationToken)
    {
        var exists = await _unitOfWork.GradeSetupRepository.NameExistsAsync(academicYearId, gradeName, excludeId, cancellationToken);

        if (exists)
        {
            throw new BadRequestException($"A grade named '{gradeName}' already exists for this academic year.");
        }
    }

    /// <summary>Ensures the given percentage range does not overlap any other active band in the same academic year.</summary>
    private async Task EnsureNoOverlapAsync(int academicYearId, decimal minPercentage, decimal maxPercentage, int? excludeId, CancellationToken cancellationToken)
    {
        var activeBands = await _unitOfWork.GradeSetupRepository.GetActiveByAcademicYearAsync(academicYearId, cancellationToken);

        var overlaps = activeBands.Any(x =>
            (!excludeId.HasValue || x.Id != excludeId.Value) &&
            minPercentage <= x.MaxPercentage &&
            maxPercentage >= x.MinPercentage);

        if (overlaps)
        {
            throw new BadRequestException("This grade band's percentage range overlaps another active grade band for the same academic year.");
        }
    }
}
