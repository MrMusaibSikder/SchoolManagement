using CourseHub.Application.Features.Teachers.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Teachers.Validators;

public class UpdateTeacherProfileImageRequestValidator : AbstractValidator<UpdateTeacherProfileImageRequest>
{
    public UpdateTeacherProfileImageRequestValidator()
    {
        // Not validated as a strict absolute URL — same reasoning as
        // Courses.UpdateCourseThumbnailRequestValidator.
        RuleFor(x => x.ProfileImageUrl)
            .MaximumLength(500);
    }
}
