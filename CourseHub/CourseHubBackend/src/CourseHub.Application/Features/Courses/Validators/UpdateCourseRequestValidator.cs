using CourseHub.Application.Features.Courses.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Courses.Validators;

public class UpdateCourseRequestValidator : AbstractValidator<UpdateCourseRequest>
{
    public UpdateCourseRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.DurationInMonths)
            .GreaterThan(0);

        RuleFor(x => x.Description)
            .MaximumLength(4000);
    }
}
