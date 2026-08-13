using AutoMapper;
using FluentValidation;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.LateFineRule.DTOs;
using SchoolERP.Application.Features.LateFineRule.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Services
{
    public class LateFineRuleService : ILateFineRuleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateLateFineRuleDto> _createValidator;
        private readonly IValidator<UpdateLateFineRuleDto> _updateValidator;

        public LateFineRuleService(
            IUnitOfWork unitOfWork, IMapper mapper,
            IValidator<CreateLateFineRuleDto> createValidator, IValidator<UpdateLateFineRuleDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IReadOnlyList<LateFineRuleDto>> GetByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default)
        {
            var rules = await _unitOfWork.LateFineRuleRepository.GetByAcademicYearAsync(academicYearId, cancellationToken);
            return _mapper.Map<IReadOnlyList<LateFineRuleDto>>(rules);
        }

        public async Task<LateFineRuleDto> CreateAsync(CreateLateFineRuleDto request, CancellationToken cancellationToken = default)
        {
            var validation = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var entity = _mapper.Map<SchoolERP.Domain.Entities.LateFineRule>(request);
            await _unitOfWork.LateFineRuleRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<LateFineRuleDto>(entity);
        }

        public async Task<LateFineRuleDto> UpdateAsync(int id, UpdateLateFineRuleDto request, CancellationToken cancellationToken = default)
        {
            if (id != request.Id) throw new BadRequestException("Route id and body id do not match.");

            var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var entity = await _unitOfWork.LateFineRuleRepository.GetByIdTrackedAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.LateFineRule), id);

            entity.Type = request.Type;
            entity.Amount = request.Amount;
            entity.GracePeriodDays = request.GracePeriodDays;
            entity.MaxFineAmount = request.MaxFineAmount;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<LateFineRuleDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.LateFineRuleRepository.GetByIdTrackedAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.LateFineRule), id);

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
