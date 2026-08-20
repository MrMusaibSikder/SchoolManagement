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
    ///
    /// Return type is the concrete ActionResult class, NOT the IActionResult
    /// interface — ActionResult&lt;T&gt; (used by GetById/Create/Update/etc.
    /// across every Phase 12 controller) only has an implicit conversion
    /// from ActionResult (and from T itself), not from IActionResult. Using
    /// IActionResult here compiles fine for the plain-IActionResult actions
    /// (e.g. Delete) but fails everywhere the caller does
    /// "return validationError;" inside an ActionResult&lt;T&gt; method.
    /// </summary>
    protected async Task<ActionResult?> ValidateAsync<T>(IValidator<T> validator, T request, CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);

        if (result.IsValid)
        {
            return null;
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        return ValidationProblem(ModelState);
    }
}
