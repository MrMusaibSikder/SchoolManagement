using CourseHub.Application.Features.Students.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Students.Validators;

public class UpdateStudentProfileImageRequestValidator : AbstractValidator<UpdateStudentProfileImageRequest>
{
    public UpdateStudentProfileImageRequestValidator()
    {
        RuleFor(x => x.ProfileImageUrl)
            .MaximumLength(500);
    }
}
