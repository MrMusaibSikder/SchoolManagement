using FluentValidation;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.StudentFeeConcession.DTOs;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.StudentFeeConcession.Validators
{
    public class CreateStudentFeeConcessionDtoValidator : AbstractValidator<CreateStudentFeeConcessionDto>
    {
        public CreateStudentFeeConcessionDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.StudentId).GreaterThan(0)
                .MustAsync(async (id, ct) => await unitOfWork.StudentRepository.ExistsAsync(id, ct))
                .WithMessage("Selected student does not exist.");

            RuleFor(x => x.FeeTypeId).GreaterThan(0)
                .MustAsync(async (id, ct) => await unitOfWork.FeeTypeRepository.ExistsAsync(id, ct))
                .WithMessage("Selected fee type does not exist.");

            RuleFor(x => x.AcademicYearId).GreaterThan(0)
                .MustAsync(async (id, ct) => await unitOfWork.AcademicYearRepository.ExistsAsync(id, ct))
                .WithMessage("Selected academic year does not exist.");

            RuleFor(x => x.Type).IsInEnum();

            RuleFor(x => x.Value)
                .NotNull().WithMessage("Value is required for this concession type.")
                .When(x => x.Type != ConcessionType.FullExemption);

            RuleFor(x => x.Value)
                .InclusiveBetween(0.01m, 100)
                .WithMessage("Percentage value must be between 0 and 100.")
                .When(x => x.Type == ConcessionType.PercentageDiscount && x.Value.HasValue);

            RuleFor(x => x.Value)
                .GreaterThan(0)
                .When(x => x.Type == ConcessionType.FixedAmountDiscount && x.Value.HasValue);

            RuleFor(x => x.Reason).NotEmpty().MaximumLength(300);

            RuleFor(x => x.ValidTo)
                .GreaterThan(x => x.ValidFrom)
                .When(x => x.ValidFrom.HasValue && x.ValidTo.HasValue)
                .WithMessage("Valid-to date must be after valid-from date.");

            RuleFor(x => x)
                .MustAsync(async (dto, ct) => !await unitOfWork.StudentFeeConcessionRepository.AnyAsync(
                    c => c.StudentId == dto.StudentId && c.FeeTypeId == dto.FeeTypeId && c.AcademicYearId == dto.AcademicYearId, ct))
                .WithMessage("A concession already exists for this student/fee type/academic year.")
                .WithName("StudentId");
        }
    }
}
