using AutoMapper;
using FluentValidation;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Receipt.DTOs;
using SchoolERP.Application.Features.Receipt.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<VoidReceiptDto> _voidValidator;

        public ReceiptService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<VoidReceiptDto> voidValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _voidValidator = voidValidator;
        }

        public async Task<ReceiptDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var receipt = await _unitOfWork.ReceiptRepository.GetWithDetailsAsync(id, cancellationToken);
            return receipt is null ? null : _mapper.Map<ReceiptDto>(receipt);
        }

        public async Task<ReceiptDto?> GetByPaymentIdAsync(int paymentId, CancellationToken cancellationToken = default)
        {
            var receipt = await _unitOfWork.ReceiptRepository.GetByPaymentIdAsync(paymentId, cancellationToken);
            return receipt is null ? null : _mapper.Map<ReceiptDto>(receipt);
        }

        /// <summary>
        ///  This is a standalone void operation. Voiding a receipt directly will not update
        /// the related invoice balance.
        /// The correct flow is to call PaymentService.VoidAsync(), which handles Payment,
        /// Receipt, and Invoice updates together.
        /// This method is kept only for specific edge cases (for example, when an incorrect
        /// receipt was reprinted but the payment record remains valid).
        /// </summary>
        public async Task VoidAsync(int id, VoidReceiptDto request, CancellationToken cancellationToken = default)
        {
            var validation = await _voidValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var entity = await _unitOfWork.ReceiptRepository.GetByIdTrackedAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.Receipt), id);

            if (entity.IsVoided)
                throw new BadRequestException("Receipt is already voided.");

            entity.IsVoided = true;
            entity.VoidedAt = DateTime.UtcNow;
            entity.VoidReason = request.VoidReason;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
