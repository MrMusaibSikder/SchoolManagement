using FluentValidation;
using SchoolERP.Application.Features.Notice.DTOs;

namespace SchoolERP.Application.Features.Notice.Validators;

/// <summary>Validation rules for <see cref="NoticeQueryDto"/>.</summary>
public class NoticeQueryDtoValidator : AbstractValidator<NoticeQueryDto>
{
    public NoticeQueryDtoValidator()
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
