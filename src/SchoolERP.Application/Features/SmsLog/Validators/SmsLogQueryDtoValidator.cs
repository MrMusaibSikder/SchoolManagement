using FluentValidation;
using SchoolERP.Application.Features.SmsLog.DTOs;

namespace SchoolERP.Application.Features.SmsLog.Validators;

/// <summary>Validation rules for <see cref="SmsLogQueryDto"/>.</summary>
public class SmsLogQueryDtoValidator : AbstractValidator<SmsLogQueryDto>
{
    public SmsLogQueryDtoValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("'ToDate' cannot be earlier than 'FromDate'.");
    }
}
