using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.ExamWeightSetup.DTOs;
using SchoolERP.Application.Features.ExamWeightSetup.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for exam weight setups and their items. Calls the
/// repositories (via the Unit of Work), applies business rules (weights must
/// total 100% to activate, only one active setup per academic year, items
/// cannot be changed once active), and maps entities to/from DTOs using
/// AutoMapper.
/// </summary>
public class ExamWeightSetupService : IExamWeightSetupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExamWeightSetupService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamWeightSetupDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.ExamWeightSetupRepository.GetAllWithItemsAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ExamWeightSetupDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<ExamWeightSetupDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamWeightSetupRepository.GetByIdWithItemsAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<ExamWeightSetupDto>(entity);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamWeightSetupDto>> GetByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.ExamWeightSetupRepository.GetByAcademicYearAsync(academicYearId, cancellationToken);
        return _mapper.Map<IReadOnlyList<ExamWeightSetupDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<ExamWeightSetupDto?> GetActiveByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamWeightSetupRepository.GetActiveByAcademicYearAsync(academicYearId, cancellationToken);
        return entity is null ? null : _mapper.Map<ExamWeightSetupDto>(entity);
    }

    /// <inheritdoc />
    public async Task<ExamWeightSetupDto> CreateAsync(CreateExamWeightSetupDto request, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.AcademicYearRepository.ExistsAsync(request.AcademicYearId, cancellationToken))
        {
            throw new NotFoundException(nameof(AcademicYear), request.AcademicYearId);
        }

        foreach (var item in request.Items)
        {
            if (!await _unitOfWork.ExamRepository.ExamExistsAsync(item.ExamId, cancellationToken))
            {
                throw new NotFoundException(nameof(Exam), item.ExamId);
            }
        }

        var entity = new ExamWeightSetup
        {
            AcademicYearId = request.AcademicYearId,
            Name = request.Name,
            IsActive = false
        };

        await _unitOfWork.ExamWeightSetupRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var item in request.Items)
        {
            await _unitOfWork.ExamWeightItemRepository.AddAsync(new ExamWeightItem
            {
                ExamWeightSetupId = entity.Id,
                ExamId = item.ExamId,
                WeightPercentage = item.WeightPercentage
            }, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(entity.Id, cancellationToken) ?? throw new NotFoundException(nameof(ExamWeightSetup), entity.Id);
    }

    /// <inheritdoc />
    public async Task<ExamWeightSetupDto> UpdateAsync(int id, UpdateExamWeightSetupDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamWeightSetupRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamWeightSetup), id);

        entity.Name = request.Name;

        _unitOfWork.ExamWeightSetupRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(ExamWeightSetup), id);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamWeightSetupRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamWeightSetup), id);

        if (entity.IsActive)
        {
            throw new BadRequestException("An active weight setup cannot be deleted. Deactivate it first.");
        }

        _unitOfWork.ExamWeightSetupRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ExamWeightSetupDto> ActivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamWeightSetupRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamWeightSetup), id);

        var totalWeight = await _unitOfWork.ExamWeightItemRepository.GetTotalWeightAsync(id, cancellationToken);

        if (totalWeight != 100m)
        {
            throw new BadRequestException($"This setup's weights total {totalWeight}%, but must total exactly 100% to be activated.");
        }

        // Deactivate any other setup currently active for the same academic year (versioning).
        var siblings = await _unitOfWork.ExamWeightSetupRepository.GetByAcademicYearAsync(entity.AcademicYearId, cancellationToken);
        foreach (var sibling in siblings.Where(x => x.IsActive && x.Id != id))
        {
            var trackedSibling = await _unitOfWork.ExamWeightSetupRepository.GetByIdTrackedAsync(sibling.Id, cancellationToken);
            if (trackedSibling is null)
                continue;

            trackedSibling.IsActive = false;
            _unitOfWork.ExamWeightSetupRepository.Update(trackedSibling);
        }

        entity.IsActive = true;
        _unitOfWork.ExamWeightSetupRepository.Update(entity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(ExamWeightSetup), id);
    }

    /// <inheritdoc />
    public async Task<ExamWeightSetupDto> DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamWeightSetupRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamWeightSetup), id);

        entity.IsActive = false;
        _unitOfWork.ExamWeightSetupRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(ExamWeightSetup), id);
    }

    /// <inheritdoc />
    public async Task<ExamWeightSetupDto> AddItemAsync(AddExamWeightItemDto request, CancellationToken cancellationToken = default)
    {
        var setup = await _unitOfWork.ExamWeightSetupRepository.GetByIdTrackedAsync(request.ExamWeightSetupId, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamWeightSetup), request.ExamWeightSetupId);

        EnsureSetupIsEditable(setup);

        if (!await _unitOfWork.ExamRepository.ExamExistsAsync(request.ExamId, cancellationToken))
        {
            throw new NotFoundException(nameof(Exam), request.ExamId);
        }

        var exists = await _unitOfWork.ExamWeightItemRepository.ExamExistsInSetupAsync(request.ExamWeightSetupId, request.ExamId, null, cancellationToken);
        if (exists)
        {
            throw new BadRequestException("This exam already has a weight item in this setup.");
        }

        await _unitOfWork.ExamWeightItemRepository.AddAsync(new ExamWeightItem
        {
            ExamWeightSetupId = request.ExamWeightSetupId,
            ExamId = request.ExamId,
            WeightPercentage = request.WeightPercentage
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(request.ExamWeightSetupId, cancellationToken) ?? throw new NotFoundException(nameof(ExamWeightSetup), request.ExamWeightSetupId);
    }

    /// <inheritdoc />
    public async Task<ExamWeightSetupDto> UpdateItemAsync(int itemId, UpdateExamWeightItemDto request, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.ExamWeightItemRepository.GetByIdTrackedAsync(itemId, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamWeightItem), itemId);

        var setup = await _unitOfWork.ExamWeightSetupRepository.GetByIdTrackedAsync(item.ExamWeightSetupId, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamWeightSetup), item.ExamWeightSetupId);

        EnsureSetupIsEditable(setup);

        item.WeightPercentage = request.WeightPercentage;
        _unitOfWork.ExamWeightItemRepository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(setup.Id, cancellationToken) ?? throw new NotFoundException(nameof(ExamWeightSetup), setup.Id);
    }

    /// <inheritdoc />
    public async Task<ExamWeightSetupDto> RemoveItemAsync(int itemId, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.ExamWeightItemRepository.GetByIdTrackedAsync(itemId, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamWeightItem), itemId);

        var setup = await _unitOfWork.ExamWeightSetupRepository.GetByIdTrackedAsync(item.ExamWeightSetupId, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamWeightSetup), item.ExamWeightSetupId);

        EnsureSetupIsEditable(setup);

        _unitOfWork.ExamWeightItemRepository.Delete(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(setup.Id, cancellationToken) ?? throw new NotFoundException(nameof(ExamWeightSetup), setup.Id);
    }

    /// <summary>Ensures the setup is not currently active before allowing its items to change.</summary>
    private static void EnsureSetupIsEditable(ExamWeightSetup setup)
    {
        if (setup.IsActive)
        {
            throw new BadRequestException("This weight setup is active and its items cannot be changed. Deactivate it first.");
        }
    }
}
