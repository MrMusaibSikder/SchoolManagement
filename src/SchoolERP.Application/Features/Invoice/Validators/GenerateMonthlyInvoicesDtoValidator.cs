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
    public class GenerateMonthlyInvoicesDtoValidator : AbstractValidator<GenerateMonthlyInvoicesDto>
    {
        public GenerateMonthlyInvoicesDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.AcademicYearId).GreaterThan(0)
                .MustAsync(async (id, ct) => await unitOfWork.AcademicYearRepository.ExistsAsync(id, ct))
                .WithMessage("Selected academic year does not exist.");

            RuleFor(x => x.Month).InclusiveBetween(1, 12);

            RuleFor(x => x.Year).GreaterThanOrEqualTo(2000).LessThanOrEqualTo(2100);

            RuleFor(x => x.SchoolClassId)
                .MustAsync(async (id, ct) => id == null || await unitOfWork.SchoolClassRepository.ExistsAsync(id.Value, ct))
                .WithMessage("Selected school class does not exist.");

            RuleFor(x => x.DueDate).NotEmpty();
        }
    }
}
