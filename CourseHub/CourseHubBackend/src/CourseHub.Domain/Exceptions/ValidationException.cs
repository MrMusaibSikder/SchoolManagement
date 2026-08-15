namespace CourseHub.Domain.Exceptions;

/// <summary>
/// Raised when a domain rule/invariant is violated.
/// </summary>
public class ValidationException : DomainException
{
    public ValidationException(string message)
        : base(message)
    {
    }
}
