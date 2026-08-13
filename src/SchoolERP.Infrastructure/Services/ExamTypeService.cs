using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.ExamType.DTOs;
using SchoolERP.Application.Features.ExamType.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for ExamType records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class ExamTypeService : IExamTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExamTypeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamTypeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.ExamTypeRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ExamTypeDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<ExamTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamTypeRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<ExamTypeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<ExamTypeDto> CreateAsync(CreateExamTypeDto request, CancellationToken cancellationToken = default)
    {
        await EnsureNameIsUniqueAsync(request.Name, excludeId: null, cancellationToken);

        var entity = _mapper.Map<ExamType>(request);

        await _unitOfWork.ExamTypeRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ExamTypeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<ExamTypeDto> UpdateAsync(int id, UpdateExamTypeDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamTypeRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamType), id);

        await EnsureNameIsUniqueAsync(request.Name, excludeId: id, cancellationToken);

        _mapper.Map(request, entity);

        _unitOfWork.ExamTypeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ExamTypeDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamTypeRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamType), id);

        var isInUse = await _unitOfWork.ExamTypeRepository.IsInUseAsync(id, cancellationToken);

        if (isInUse)
        {
            throw new BadRequestException("This exam type is used by one or more exams and cannot be deleted.");
        }

        _unitOfWork.ExamTypeRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Ensures no other (non-deleted) exam type already uses the given name (case-insensitive).</summary>
    private async Task EnsureNameIsUniqueAsync(string name, int? excludeId, CancellationToken cancellationToken)
    {
        var exists = await _unitOfWork.ExamTypeRepository.NameExistsAsync(name, excludeId, cancellationToken);

        if (exists)
        {
            throw new BadRequestException($"An exam type named '{name}' already exists.");
        }
    }
}
