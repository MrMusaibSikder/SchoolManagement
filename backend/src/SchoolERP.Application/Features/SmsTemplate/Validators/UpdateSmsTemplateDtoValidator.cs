using FluentValidation;
using SchoolERP.Application.Features.SmsTemplate.DTOs;

namespace SchoolERP.Application.Features.SmsTemplate.Validators;

/// <summary>Validation rules for <see cref="UpdateSmsTemplateDto"/>.</summary>
public class UpdateSmsTemplateDtoValidator : AbstractValidator<UpdateSmsTemplateDto>
{
    public UpdateSmsTemplateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("A valid template id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Template name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Template message is required.")
            .MaximumLength(500);
    }
}
