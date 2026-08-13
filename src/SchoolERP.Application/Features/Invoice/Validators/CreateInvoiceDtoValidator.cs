using FluentValidation;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Invoice.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Invoice.Validators
{
    public class CreateInvoiceDtoValidator : AbstractValidator<CreateInvoiceDto>
    {
        public CreateInvoiceDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.StudentId).GreaterThan(0)
                .MustAsync(async (id, ct) => await unitOfWork.StudentRepository.ExistsAsync(id, ct))
                .WithMessage("Selected student does not exist.");

            RuleFor(x => x.AcademicYearId).GreaterThan(0)
                .MustAsync(async (id, ct) => await unitOfWork.AcademicYearRepository.ExistsAsync(id, ct))
                .WithMessage("Selected academic year does not exist.");

            RuleFor(x => x.FeeStructureId)
                .MustAsync(async (id, ct) => id == null || await unitOfWork.FeeStructureRepository.ExistsAsync(id.Value, ct))
                .WithMessage("Selected fee structure does not exist.");

            RuleFor(x => x.InvoiceDate).NotEmpty();

            RuleFor(x => x.DueDate)
                .GreaterThanOrEqualTo(x => x.InvoiceDate)
                .WithMessage("Due date cannot be earlier than the invoice date.");

            RuleFor(x => x.Month).InclusiveBetween(1, 12).When(x => x.Month.HasValue);

            RuleFor(x => x.Notes).MaximumLength(500);

            RuleFor(x => x.Items).NotEmpty().WithMessage("An invoice must have at least one item.");

            RuleForEach(x => x.Items).SetValidator(new CreateInvoiceItemDtoValidator());

            RuleFor(x => x)
                .MustAsync(async (dto, ct) =>
                {
                    if (dto.FeeStructureId is null) return true;
                    return !await unitOfWork.InvoiceRepository.ExistsForPeriodAsync(
                        dto.StudentId, dto.AcademicYearId, dto.Month, dto.Year, dto.FeeStructureId.Value, ct);
                })
                .WithMessage("An invoice already exists for this student/period/fee structure.")
                .WithName("StudentId");
        }
    }
}
