using FluentValidation;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.StudentFeeConcession.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.StudentFeeConcession.Validators
{
    public class ApproveConcessionDtoValidator : AbstractValidator<ApproveConcessionDto>
    {
        public ApproveConcessionDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.ConcessionId).GreaterThan(0)
                .MustAsync(async (id, ct) => await unitOfWork.StudentFeeConcessionRepository.ExistsAsync(id, ct))
                .WithMessage("Concession record does not exist.");
        }
    }

}
