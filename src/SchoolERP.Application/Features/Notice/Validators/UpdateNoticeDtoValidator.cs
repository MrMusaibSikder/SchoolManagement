using FluentValidation;
using SchoolERP.Application.Features.Notice.DTOs;

namespace SchoolERP.Application.Features.Notice.Validators;

/// <summary>Validation rules for <see cref="UpdateNoticeDto"/>.</summary>
public class UpdateNoticeDtoValidator : AbstractValidator<UpdateNoticeDto>
{
    public UpdateNoticeDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("A valid notice id is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000);

        RuleFor(x => x.PublishDate)
            .NotEmpty().WithMessage("Publish date is required.");

        RuleFor(x => x.ExpiryDate)
            .GreaterThanOrEqualTo(x => x.PublishDate)
            .When(x => x.ExpiryDate.HasValue)
            .WithMessage("Expiry date cannot be earlier than the publish date.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("A valid priority is required.");

        RuleFor(x => x.Audience)
            .IsInEnum().WithMessage("A valid audience is required.");
    }
}
