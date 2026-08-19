using CourseHub.Application.Features.Batches.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Batches.Validators;

public class UpdateBatchRequestValidator : AbstractValidator<UpdateBatchRequest>
{
    public UpdateBatchRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);
    }
}
