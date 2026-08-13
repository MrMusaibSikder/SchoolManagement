using FluentValidation;
using SchoolERP.Application.Features.SmsTemplate.DTOs;

namespace SchoolERP.Application.Features.SmsTemplate.Validators;

/// <summary>Validation rules for <see cref="SmsTemplateQueryDto"/>.</summary>
public class SmsTemplateQueryDtoValidator : AbstractValidator<SmsTemplateQueryDto>
{
    public SmsTemplateQueryDtoValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
