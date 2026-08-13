using FluentValidation;
using SchoolERP.Application.Features.SmsLog.DTOs;

namespace SchoolERP.Application.Features.SmsLog.Validators;

/// <summary>Validation rules for <see cref="CreateSmsLogDto"/>.</summary>
public class CreateSmsLogDtoValidator : AbstractValidator<CreateSmsLogDto>
{
    public CreateSmsLogDtoValidator()
    {
        RuleFor(x => x.RecipientNumber)
            .NotEmpty().WithMessage("Recipient number is required.")
            .MaximumLength(20);

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(500);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("A valid SMS status is required.");

        RuleFor(x => x.Provider)
            .MaximumLength(50);

        RuleFor(x => x.ProviderResponse)
            .MaximumLength(1000);
    }
}
