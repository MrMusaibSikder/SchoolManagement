namespace CourseHub.Domain.Exceptions;

/// <summary>
/// Raised for authentication failures (invalid credentials, invalid/expired
/// token, inactive account, etc). Deliberately generic — callers should
/// generally surface a single, non-enumerable message such as "Invalid
/// email or password." regardless of the specific underlying reason.
/// </summary>
public class AuthenticationException : DomainException
{
    public AuthenticationException(string message)
        : base(message)
    {
    }
}
