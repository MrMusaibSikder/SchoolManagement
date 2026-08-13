using AutoMapper;
using FluentValidation;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.FeeStructure.DTOs;
using SchoolERP.Application.Features.FeeStructure.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for FeeStructure records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class FeeStructureService : IFeeStructureService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateFeeStructureDto> _createValidator;
    private readonly IValidator<UpdateFeeStructureDto> _updateValidator;

    public FeeStructureService(
        IUnitOfWork unitOfWork, IMapper mapper,
        IValidator<CreateFeeStructureDto> createValidator, IValidator<UpdateFeeStructureDto> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<FeeStructureListDto>> GetListAsync(
        int? academicYearId, int? schoolClassId, bool? isActive, CancellationToken cancellationToken = default)
    {
        var structures = await _unitOfWork.FeeStructureRepository.GetListAsync(academicYearId, schoolClassId, isActive, cancellationToken);
        return structures.Select(s => new FeeStructureListDto
        {
            Id = s.Id,
            Name = s.Name,
            AcademicYearName = s.AcademicYear.Name,
            SchoolClassName = s.SchoolClass.Name,
            SectionName = s.Section?.Name,
            IsTemplate = s.IsTemplate,
            IsActive = s.IsActive,
            ItemCount = s.FeeStructureItems.Count,
            TotalAmount = s.FeeStructureItems.Sum(i => i.Amount),
            EffectiveFrom = s.EffectiveFrom,
            EffectiveTo = s.EffectiveTo,
            CreatedAt = s.CreatedAt
        }).ToList();
    }

    public async Task<FeeStructureDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var structure = await _unitOfWork.FeeStructureRepository.GetWithItemsAsync(id, cancellationToken);
        return structure is null ? null : _mapper.Map<FeeStructureDto>(structure);
    }

    public async Task<FeeStructureDto> CreateAsync(CreateFeeStructureDto request, CancellationToken cancellationToken = default)
    {
        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) throw new ValidationException(validation.Errors);

        var entity = new SchoolERP.Domain.Entities.FeeStructure
        {
            Name = request.Name,
            Description = request.Description,
            AcademicYearId = request.AcademicYearId,
            SchoolClassId = request.SchoolClassId,
            SectionId = request.SectionId,
            IsActive = true,
            IsTemplate = request.IsTemplate,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            ClonedFromId = request.ClonedFromId,
            FeeStructureItems = request.Items.Select(i => new FeeStructureItem
            {
                FeeTypeId = i.FeeTypeId,
                Amount = i.Amount,
                IsOptional = i.IsOptional,
                SortOrder = i.SortOrder
            }).ToList()
        };

        await _unitOfWork.FeeStructureRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _unitOfWork.FeeStructureRepository.GetWithItemsAsync(entity.Id, cancellationToken);
        return _mapper.Map<FeeStructureDto>(created);
    }

    /// <summary>
    /// Performs item-level merging:
    /// - Id == null: adds a new item.
    /// - Id is provided and IsDeleted == true: soft-deletes the existing item.
    /// - Id is provided and IsDeleted == false: updates the existing item's amount and sort order.
    ///
    /// The collection is not replaced using AutoMapper. This preserves existing
    /// FeeStructureItem.Id values, which may be referenced by invoices, audit logs,
    /// reports, or other historical records.
    /// </summary>
    public async Task<FeeStructureDto> UpdateAsync(int id, UpdateFeeStructureDto request, CancellationToken cancellationToken = default)
    {
        if (id != request.Id) throw new BadRequestException("Route id and body id do not match.");

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) throw new ValidationException(validation.Errors);

        var entity = await _unitOfWork.FeeStructureRepository.GetWithItemsTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.FeeStructure), id);

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.SectionId = request.SectionId;
        entity.IsActive = request.IsActive;
        entity.IsTemplate = request.IsTemplate;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.EffectiveTo = request.EffectiveTo;
        entity.UpdatedAt = DateTime.UtcNow;

        foreach (var itemDto in request.Items)
        {
            if (itemDto.Id is null)
            {
                // new item
                entity.FeeStructureItems.Add(new FeeStructureItem
                {
                    FeeStructureId = entity.Id,
                    FeeTypeId = itemDto.FeeTypeId,
                    Amount = itemDto.Amount,
                    IsOptional = itemDto.IsOptional,
                    SortOrder = itemDto.SortOrder
                });
                continue;
            }

            var existingItem = entity.FeeStructureItems.FirstOrDefault(x => x.Id == itemDto.Id);
            if (existingItem is null)
                throw new NotFoundException(nameof(FeeStructureItem), itemDto.Id.Value);

            if (itemDto.IsDeleted)
            {
                existingItem.IsDeleted = true;
                existingItem.DeletedAt = DateTime.UtcNow;
            }
            else
            {
                existingItem.Amount = itemDto.Amount;
                existingItem.IsOptional = itemDto.IsOptional;
                existingItem.SortOrder = itemDto.SortOrder;
                existingItem.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _unitOfWork.FeeStructureRepository.GetWithItemsAsync(id, cancellationToken);
        return _mapper.Map<FeeStructureDto>(updated);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FeeStructureRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.FeeStructure), id);

        // Business rule: Do not delete if invoices have already been generated.
        // Deactivating the record is the safer option.
        var hasInvoices = await _unitOfWork.InvoiceRepository.AnyAsync(x => x.FeeStructureId == id, cancellationToken);
        if (hasInvoices)
            throw new BadRequestException("Cannot delete a fee structure that has generated invoices. Deactivate it instead.");

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
