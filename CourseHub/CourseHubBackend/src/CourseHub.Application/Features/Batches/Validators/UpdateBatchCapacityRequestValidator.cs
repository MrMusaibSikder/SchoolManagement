using CourseHub.Application.Features.Batches.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Batches.Validators;

public class UpdateBatchCapacityRequestValidator : AbstractValidator<UpdateBatchCapacityRequest>
{
    public UpdateBatchCapacityRequestValidator()
    {
        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .When(x => x.Capacity.HasValue);
    }
}
