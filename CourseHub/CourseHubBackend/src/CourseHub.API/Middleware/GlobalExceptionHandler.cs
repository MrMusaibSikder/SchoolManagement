using System.Diagnostics;
using CourseHub.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Middleware;

/// <summary>
/// Phase 10: global exception handling. Replaces the interim
/// DomainExceptionFilter — every unhandled exception thrown anywhere in
/// the request pipeline (controllers, model binding, middleware,
/// Application/Infrastructure code) lands here exactly once and is
/// turned into a consistent RFC 7807 ProblemDetails response instead of
/// a bare 500 or a framework-default HTML error page.
///
/// Registered via builder.Services.AddExceptionHandler&lt;GlobalExceptionHandler&gt;()
/// + app.UseExceptionHandler() in Program.cs. .NET 8's IExceptionHandler
/// pipeline supports multiple handlers tried in registration order; this
/// is currently the only one, acting as a catch-all.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        IHostEnvironment environment,
        ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _environment = environment;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = MapException(exception);

        // Server errors (unmapped exceptions) are logged with full detail
        // server-side, always — regardless of environment. Client errors
        // (4xx, e.g. "email already exists") are expected, recoverable
        // conditions, not application bugs, so they're logged at a lower
        // level to keep error-level logs meaningful/actionable.
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception processing {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "{ExceptionType} while processing {Method} {Path}: {Message}", exception.GetType().Name, httpContext.Request.Method, httpContext.Request.Path, exception.Message);
        }

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            // Never surface raw exception details for unmapped (500)
            // exceptions to the client — only the safe, generic title.
            // Domain exceptions (400/401/404) carry an intentional,
            // caller-facing message written by the Application/Domain
            // layer, so those are safe to return as-is.
            Detail = statusCode == StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred. Please try again later."
                : exception.Message,
            Instance = httpContext.Request.Path,
        };

        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        // Stack traces are dev-only convenience, never sent in any other
        // environment (Staging/Production), even for unmapped 500s.
        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["exceptionType"] = exception.GetType().FullName;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails,
        });
    }

    private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
    {
        AuthenticationException => (StatusCodes.Status401Unauthorized, "Authentication Failed"),
        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
        NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
        ValidationException => (StatusCodes.Status400BadRequest, "Validation Error"),
        FluentValidation.ValidationException => (StatusCodes.Status400BadRequest, "Validation Error"),
        DomainException => (StatusCodes.Status400BadRequest, "Bad Request"),
        _ => (StatusCodes.Status500InternalServerError, "Internal Server Error"),
    };
}
