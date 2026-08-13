using FluentValidation;
using SchoolERP.Application.Features.SmsTemplate.DTOs;

namespace SchoolERP.Application.Features.SmsTemplate.Validators;

/// <summary>Validation rules for <see cref="CreateSmsTemplateDto"/>.</summary>
public class CreateSmsTemplateDtoValidator : AbstractValidator<CreateSmsTemplateDto>
{
    public CreateSmsTemplateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Template name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Template message is required.")
            .MaximumLength(500);
    }
}
