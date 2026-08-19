using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers;

/// <summary>
/// Shared base for admin controllers that validate a request with
/// FluentValidation before calling into the Application layer. Centralizes
/// the validate -> ModelState -> ValidationProblem() conversion so every
/// new Phase 12 controller (Teachers, Students, Batches, Enrollments...)
/// gets the same behavior for free instead of re-copying it.
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Validates <paramref name="request"/>. Returns null when valid (the
    /// caller should proceed); returns a ready-to-return 400
    /// ValidationProblem ActionResult when invalid.
    /// </summary>
    protected async Task<bool> ValidateAsync<T>(IValidator<T> validator, T request, CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);

        if (result.IsValid)
        {
            return true;
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        return false;
    }

    /// <summary>
    /// Returns a standardized HTTP 400 validation response.
    /// </summary>
    protected ActionResult ValidationError()
    {
        return ValidationProblem(ModelState);
    }
}
