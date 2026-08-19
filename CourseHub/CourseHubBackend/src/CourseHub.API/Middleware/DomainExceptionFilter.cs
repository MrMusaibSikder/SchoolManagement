using CourseHub.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CourseHub.API.Middleware;

/// <summary>
/// Interim, narrow exception-to-status-code mapping so authentication
/// endpoints return sane HTTP responses (400/401/404) instead of an
/// unhandled 500. This is intentionally NOT a full global exception
/// handling framework — Phase 10 will introduce ProblemDetails-based
/// global exception handling and this filter should be retired/absorbed
/// into it then. Registered globally via AddControllers(options =>
/// options.Filters.Add&lt;DomainExceptionFilter&gt;()) in Program.cs since no
/// other controllers exist yet to be affected differently.
/// </summary>
public class DomainExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var (statusCode, message) = context.Exception switch
        {
            AuthenticationException ex => (StatusCodes.Status401Unauthorized, ex.Message),
            NotFoundException ex => (StatusCodes.Status404NotFound, ex.Message),
            ValidationException ex => (StatusCodes.Status400BadRequest, ex.Message),
            DomainException ex => (StatusCodes.Status400BadRequest, ex.Message),
            _ => (0, string.Empty),
        };

        if (statusCode == 0)
        {
            // Not a recognized domain exception — let it propagate as an
            // unhandled 500. Never surface raw exception details to the
            // client.
            return;
        }

        context.Result = new ObjectResult(new { status = statusCode, message })
        {
            StatusCode = statusCode,
        };

        context.ExceptionHandled = true;
    }
}
