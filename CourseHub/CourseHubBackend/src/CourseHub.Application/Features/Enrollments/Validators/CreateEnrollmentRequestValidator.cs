using CourseHub.Application.Features.Enrollments.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Enrollments.Validators;

public class CreateEnrollmentRequestValidator : AbstractValidator<CreateEnrollmentRequest>
{
    public CreateEnrollmentRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty();

        RuleFor(x => x.BatchId)
            .NotEmpty();
    }
}
