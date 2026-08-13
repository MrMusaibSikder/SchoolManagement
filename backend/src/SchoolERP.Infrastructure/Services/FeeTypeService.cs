using AutoMapper;
using FluentValidation;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.FeeType.DTOs;
using SchoolERP.Application.Features.FeeType.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for FeeType records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class FeeTypeService : IFeeTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateFeeTypeDto> _createValidator;
    private readonly IValidator<UpdateFeeTypeDto> _updateValidator;

    public FeeTypeService(
        IUnitOfWork unitOfWork, IMapper mapper,
        IValidator<CreateFeeTypeDto> createValidator, IValidator<UpdateFeeTypeDto> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<FeeTypeListDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var types = await _unitOfWork.FeeTypeRepository.GetAllWithCategoryAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<FeeTypeListDto>>(types);
    }

    public async Task<FeeTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var type = await _unitOfWork.FeeTypeRepository.GetWithCategoryAsync(id, cancellationToken);
        return type is null ? null : _mapper.Map<FeeTypeDto>(type);
    }

    public async Task<FeeTypeDto> CreateAsync(CreateFeeTypeDto request, CancellationToken cancellationToken = default)
    {
        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) throw new ValidationException(validation.Errors);

        var entity = _mapper.Map<SchoolERP.Domain.Entities.FeeType>(request);
        await _unitOfWork.FeeTypeRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _unitOfWork.FeeTypeRepository.GetWithCategoryAsync(entity.Id, cancellationToken);
        return _mapper.Map<FeeTypeDto>(created);
    }

    public async Task<FeeTypeDto> UpdateAsync(int id, UpdateFeeTypeDto request, CancellationToken cancellationToken = default)
    {
        if (id != request.Id) throw new BadRequestException("Route id and body id do not match.");

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) throw new ValidationException(validation.Errors);

        var entity = await _unitOfWork.FeeTypeRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.FeeType), id);

        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Description = request.Description;
        entity.FeeCategoryId = request.FeeCategoryId;
        entity.Frequency = request.Frequency;
        entity.IsMandatory = request.IsMandatory;
        entity.IsRefundable = request.IsRefundable;
        entity.IsActive = request.IsActive;
        entity.DefaultDueDayOfMonth = request.DefaultDueDayOfMonth;
        entity.DefaultGracePeriodDays = request.DefaultGracePeriodDays;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _unitOfWork.FeeTypeRepository.GetWithCategoryAsync(id, cancellationToken);
        return _mapper.Map<FeeTypeDto>(updated);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FeeTypeRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.FeeType), id);

        // Business rule: Do not delete if this fee type is used by FeeStructureItem or InvoiceItem
        // to preserve historical data.
        var inUse = await _unitOfWork.FeeTypeRepository.AnyAsync(
            x => x.Id == id && (x.FeeStructureItems.Any() || x.InvoiceItems.Any()), cancellationToken);
        if (inUse)
            throw new BadRequestException("Cannot delete a fee type that is already used in fee structures or invoices. Deactivate it instead.");

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}