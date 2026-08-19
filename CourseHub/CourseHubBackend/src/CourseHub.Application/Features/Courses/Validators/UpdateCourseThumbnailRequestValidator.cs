using CourseHub.Application.Features.Courses.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Courses.Validators;

public class UpdateCourseThumbnailRequestValidator : AbstractValidator<UpdateCourseThumbnailRequest>
{
    public UpdateCourseThumbnailRequestValidator()
    {
        // Deliberately not validated as a strict absolute URL — the
        // stored value may be a relative path (e.g. "/uploads/x.png")
        // depending on where images end up being hosted, so this only
        // guards against exceeding the DB column length
        // (CourseConfiguration: ThumbnailUrl has MaxLength(500)).
        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(500);
    }
}
