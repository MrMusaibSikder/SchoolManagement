using AutoMapper;
using FluentValidation;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.FeeCategory.DTOs;
using SchoolERP.Application.Features.FeeCategory.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Services
{
    public class FeeCategoryService : IFeeCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateFeeCategoryDto> _createValidator;
        private readonly IValidator<UpdateFeeCategoryDto> _updateValidator;

        public FeeCategoryService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CreateFeeCategoryDto> createValidator,
            IValidator<UpdateFeeCategoryDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IReadOnlyList<FeeCategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _unitOfWork.FeeCategoryRepository.GetActiveOrderedAsync(cancellationToken);
            return _mapper.Map<IReadOnlyList<FeeCategoryDto>>(categories);
        }

        public async Task<FeeCategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.FeeCategoryRepository.GetByIdAsync(id, cancellationToken);
            return category is null ? null : _mapper.Map<FeeCategoryDto>(category);
        }

        public async Task<FeeCategoryDto> CreateAsync(CreateFeeCategoryDto request, CancellationToken cancellationToken = default)
        {
            var validation = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var entity = _mapper.Map<SchoolERP.Domain.Entities.FeeCategory>(request);
            await _unitOfWork.FeeCategoryRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<FeeCategoryDto>(entity);
        }

        public async Task<FeeCategoryDto> UpdateAsync(int id, UpdateFeeCategoryDto request, CancellationToken cancellationToken = default)
        {
            if (id != request.Id)
                throw new BadRequestException("Route id and body id do not match.");

            var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            // Retrieve the tracked entity. We do not use GenericRepository.Update() here;
            // instead, we update the properties directly and rely on EF Core ChangeTracker
            // to generate a partial UPDATE statement.
             var entity = await _unitOfWork.FeeCategoryRepository.GetByIdTrackedAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.FeeCategory), id);

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.DisplayOrder = request.DisplayOrder;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<FeeCategoryDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.FeeCategoryRepository.GetByIdTrackedAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.FeeCategory), id);

            // Business rule: A fee category cannot be deleted if it has associated fee types.
            var hasFeeTypes = await _unitOfWork.FeeTypeRepository.AnyAsync(x => x.FeeCategoryId == id, cancellationToken);
            if (hasFeeTypes)
                throw new BadRequestException("Cannot delete a fee category that has associated fee types.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
