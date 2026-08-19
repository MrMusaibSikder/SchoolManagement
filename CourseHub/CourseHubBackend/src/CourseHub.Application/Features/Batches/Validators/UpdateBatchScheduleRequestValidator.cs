using CourseHub.Application.Features.Batches.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Batches.Validators;

public class UpdateBatchScheduleRequestValidator : AbstractValidator<UpdateBatchScheduleRequest>
{
    public UpdateBatchScheduleRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty();

        // The StartDate <= EndDate ordering rule itself lives in the
        // domain (Batch.SetSchedule throws a domain ValidationException,
        // mapped to 400 by GlobalExceptionHandler) rather than being
        // duplicated here — one source of truth for that business rule.
    }
}
